using MOBAManager.Management;
using MOBAManager.Management.Heroes;
using MOBAManager.Management.Players;
using MOBAManager.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MOBAManager.MatchResolution
{
    partial class MatchAI : IDisposable
    {
        #region General Variables
        /// <summary>
        /// The dictionary containing all heroes in the game.
        /// </summary>
        private Dictionary<int, Hero> allHeroes;

        /// <summary>
        /// The match that this AI is controlling.
        /// </summary>
        private Match match;
        #endregion

        #region Team-based Variables
        /// <summary>
        /// The players on Team 1.
        /// </summary>
        private List<Player> team1Players;

        /// <summary>
        /// The players on Team 2.
        /// </summary>
        private List<Player> team2Players;

        /// <summary>
        /// The heroes left to be picked or banned in the pool.
        /// </summary>
        private List<int> remainingHeroes;

        /// <summary>
        /// The list of hero picks by Team 1.
        /// </summary>
        private List<int> team1Picks;

        /// <summary>
        /// The list of hero picks by Team 2.
        /// </summary>
        private List<int> team2Picks;

        /// <summary>
        /// The list of banned heroes by Team 1.
        /// </summary>
        private List<int> team1Bans;

        /// <summary>
        /// The list of banned heroes by Team 2.
        /// </summary>
        private List<int> team2Bans;

        /// <summary>
        /// The combinations of players and heroes for Team 1.
        /// </summary>
        private List<Tuple<Player, Hero>> team1Lineup;

        /// <summary>
        /// The combinations of players and heroes for Team 2.
        /// </summary>
        private List<Tuple<Player, Hero>> team2Lineup;

        /// <summary>
        /// <para>The selection mode that Team 1 is currently using.
        /// 
        /// The pick numbers are as follows:</para>
        /// <para>-1 is a random selection.</para>
        /// <para>0 is a random selection method.</para>
        /// <para>1 is to select the best hero in the pool overall.</para>
        /// <para>2 is to select the best hero in the pool for the current player target.</para>
        /// <para>3 is to select the worst overall choice for the team.</para>
        /// <para>4 is to select the best counter to the opposing team.</para>
        /// </summary>
        private int team1SelectionMode = 0;

        /// <summary>
        /// The current player that Team 1 is currently picking for or banning against.
        /// </summary>
        private int team1SelectionPlayerTarget = -1;

        /// <summary>
        /// <para>The selection mode that Team 2 is currently using.
        /// 
        /// The pick numbers are as follows:</para>
        /// <para>-1 is a random selection.</para>
        /// <para>0 is a random selection method.</para>
        /// <para>1 is to select the best hero in the pool overall.</para>
        /// <para>2 is to select the best hero in the pool for the current player target.</para>
        /// <para>3 is to select the worst overall choice for the team.</para>
        /// <para>4 is to select the best counter to the opposing team.</para>
        /// </summary>
        private int team2SelectionMode = 0;

        /// <summary>
        /// The current player that Team 2 is currently picking for or banning against.
        /// </summary>
        private int team2SelectionPlayerTarget = -1;
        #endregion

        #region Selection Methods
        /// <summary>
        /// Returns the player with the supplied ID, or null if the player does not exist in the current match-up.
        /// </summary>
        /// <returns>The Player object being looked for.</returns>
        private Player getPlayerFromTeams(int player)
        {
            foreach (Player p in team1Players)
            {
                if (p.ID == player)
                {
                    return p;
                }
            }
            foreach (Player p in team2Players)
            {
                if (p.ID == player)
                {
                    return p;
                }
            }
            return null;
        }

        /// <summary>
        /// This creates the baseline list of hero values for selection based on the team provided.
        /// </summary>
        /// <param name="team">1 or 2, depending on which team should be picked for.</param>
        /// <param name="ignoreFriendlies"></param>
        /// <param name="ignoreEnemies"></param>
        /// <returns>A dictionary with hero IDs as the key and their skill levels as the value.</returns>
        private Dictionary<int, int> baselineHeroSelection(int team, bool ignoreFriendlies, bool ignoreEnemies)
        {
            if (team > 2 || team < 1)
            {
                throw new System.Exception("MatchAI - Trying to calculate picks for a team that does not exist.");
            }

            Dictionary<int, int> ret = new Dictionary<int, int>();

            List<int> friendlies = team1Picks;
            List<int> enemies = team2Picks;
            if (team == 2)
            {
                friendlies = team2Picks;
                enemies = team1Picks;
            }

            if (ignoreFriendlies)
            {
                friendlies = new List<int>();
            }

            if (ignoreEnemies)
            {
                enemies = new List<int>();
            }

            foreach (int key in remainingHeroes)
            {
                int value = allHeroes[key].calculatePerformance(friendlies, enemies);
                ret.Add(key, value);
            }

            return ret;
        }

        /// <summary>
        /// This creates the baseline list of hero values for selection based on the team provided.
        /// </summary>
        /// <param name="team">1 or 2, depending on which team should be picked for.</param>
        /// <returns></returns>
        private Dictionary<int, int> baselineHeroSelection(int team)
        {
            return baselineHeroSelection(team, false, false);
        }

        /// <summary>
        /// Selects a hero for a team based on the overall situation.
        /// This selection mainly relies on hero skills, synergies, and counters.
        /// Internally, the top 3 heroes are found and then randomly one is selected and returned.
        /// </summary>
        /// <param name="team">The team to choose for.</param>
        /// <returns></returns>
        private int selectBestOverallChoice(int team)
        {
            Dictionary<int, int> baseline = baselineHeroSelection(team);
            List<int> finalists = baseline.OrderByDescending(kvp => kvp.Value).Take(3).Select(kvp => kvp.Key).ToList();
            int rando = RNG.roll(6);
            if (rando < 3)
            {
                return finalists[0];
            }
            else if (rando < 5)
            {
                return finalists[1];
            }
            else
            {
                return finalists[2];
            }
        }

        /// <summary>
        /// Selects a hero for a team based on the provided player's skill levels.
        /// This selection uses the main selection criteria as a basis, then uses the player's skills as an additional parameter for determining the top choices.
        /// Internally, the top 3 heroes are found and then randomly one is selected and returned.
        /// </summary>
        /// <param name="team">The team to choose for.</param>
        /// <param name="player">The player to use as a comparison.</param>
        /// <returns></returns>
        private int selectBestChoiceForPlayer(int team, int player)
        {
            Player actualPlayer = getPlayerFromTeams(player);
            if (actualPlayer != null)
            {
                Dictionary<int, int> baseline = baselineHeroSelection(team);
                List<int> finalists = baseline.OrderByDescending(kvp => kvp.Value + actualPlayer.getHeroSkill(kvp.Key)).Take(3).Select(kvp => kvp.Key).ToList();
                int rando = RNG.roll(6);
                if (rando < 3)
                {
                    return finalists[0];
                }
                else if (rando < 5)
                {
                    return finalists[1];
                }
                else
                {
                    return finalists[2];
                }
            }
            else
            {
                return selectBestOverallChoice(team);
            }

        }

        /// <summary>
        /// Selects the worst possible hero for a team.
        /// This selection mainly relies on hero skills, synergies, and counters.
        /// Internally, the top 3 heroes are found and then randomly one is selected and returned.
        /// </summary>
        /// <param name="team">The team to select the hero for.</param>
        /// <returns></returns>
        private int selectWorstOverallChoice(int team)
        {
            Dictionary<int, int> baseline = baselineHeroSelection(team);
            List<int> finalists = baseline.OrderBy(kvp => kvp.Value).Take(3).Select(kvp => kvp.Key).ToList();
            int rando = RNG.roll(6);
            if (rando < 3)
            {
                return finalists[0];
            }
            else if (rando < 5)
            {
                return finalists[1];
            }
            else
            {
                return finalists[2];
            }
        }

        /// <summary>
        /// Selects a hero based on how well they counter a specific team line-up.
        /// This selection mainly relies on counters and hero skills, ignoring synergies.
        /// </summary>
        /// <param name="team">The team to find a counter for.</param>
        /// <returns></returns>
        private int selectBestCounterToTeam(int team)
        {
            Dictionary<int, int> baseline = baselineHeroSelection(team, true, false);
            List<int> finalists = baseline.OrderByDescending(kvp => kvp.Value).Take(3).Select(kvp => kvp.Key).ToList();
            int rando = RNG.roll(6);
            if (rando < 3)
            {
                return finalists[0];
            }
            else if (rando < 5)
            {
                return finalists[1];
            }
            else
            {
                return finalists[2];
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Returns a list of all banned heroes for a specific team.
        /// </summary>
        /// <param name="team">The team to get bans for.</param>
        /// <returns></returns>
        public List<Hero> getTeamBans(int team)
        {
            if (team > 2 || team < 1)
            {
                throw new System.Exception("MatchAI - Trying to get picks for a team that does not exist.");
            }

            List<int> bans = team1Bans;
            if (team == 2)
            {
                bans = team2Bans;
            }
            List<Hero> ret = new List<Hero>();
            foreach (int i in bans)
            {
                ret.Add(allHeroes[i]);
            }
            return ret;
        }

        /// <summary>
        /// Returns the picks for the team selected.
        /// </summary>
        /// <param name="team">The team to get picks for.</param>
        /// <returns></returns>
        public List<Hero> getTeamSelections(int team)
        {
            
            if (team > 2 || team < 1)
            {
                throw new System.Exception("MatchAI - Trying to get picks for a team that does not exist.");
            }

            List<int> selections = team1Picks;
            if (team == 2)
            {
                selections = team2Picks;
            }
            List<Hero> ret = new List<Hero>();
            foreach (int i in selections)
            {
                ret.Add(allHeroes[i]);
            }
            return ret;
        }

        /// <summary>
        /// Returns a formatted string containing all of the team's players and which heroes they're playing.
        /// </summary>
        /// <param name="team"></param>
        /// <returns></returns>
        public string getFormattedTeamLineup(int team)
        {
            string ret = "";
            List<Tuple<Player, Hero>> lineup = team1Lineup;
            if (team == 2)
            {
                lineup = team2Lineup;
            }

            foreach (Tuple<Player, Hero> tp in lineup)
            {
                ret += tp.Item1.playerName + " - " + tp.Item2.HeroName + Environment.NewLine;
            }

            return ret;
        }

        /// <summary>
        /// Sets a team's selection for either a pick or ban.
        /// </summary>
        /// <param name="team">The team making the selection.</param>
        /// <param name="heroID">The ID of the hero being selected.</param>
        public void setTeamSelection(int team, int heroID)
        {
            setTeamSelection(team, heroID, true);
        }

        /// <summary>
        /// Sets a team's selection for either a pick or ban.
        /// </summary>
        /// <param name="team">The team making the selection.</param>
        /// <param name="heroID">The ID of the hero being selected.</param>
        /// <param name="isPick">If false, this means that this is a ban and not a pick. This defaults to true.</param>
        public void setTeamSelection(int team, int heroID, bool isPick)
        {
            if (team > 2 || team < 1 || !remainingHeroes.Contains(heroID))
            {
                throw new Exception("MatchAI - Attempted to set invalid hero selection with the following parameters: " + team + "," + isPick + "," + heroID);
            }

            if (team == 1)
            {
                if (isPick)
                {
                    team1Picks.Add(heroID);
                }
                else
                {
                    team1Bans.Add(heroID);
                }
            }
            else
            {
                if (isPick)
                {
                    team2Picks.Add(heroID);
                }
                else
                {
                    team2Bans.Add(heroID);
                }
            }

            remainingHeroes.Remove(heroID);
        }

        /// <summary>
        /// Resolves a picking selection for Team 1. Note that this does not actually select anything, but instead returns what the team thinks their best pick is.
        /// </summary>
        public int team1Pick()
        {
            Dictionary<int, int> start = baselineHeroSelection(1);
            int selectionID = -1;
            switch (team1SelectionMode)
            {
                case -1:    //Random selection done due to time running out.
                    selectionID = remainingHeroes[RNG.roll(remainingHeroes.Count)];
                    break;
                case 0:     //Random method of selection.
                    if (RNG.roll(2) == 1)
                    {
                        if (RNG.roll(2) == 1)
                        {
                            if (RNG.roll(4) == 1)
                            {
                                selectionID = selectWorstOverallChoice(1);
                            }
                            else
                            {
                                selectionID = selectBestCounterToTeam(2);
                            }
                        }
                        else
                        {
                            if (RNG.roll(2) == 1)
                            {
                                int randomPlayer = team1Players[RNG.roll(team1Players.Count)].ID;
                                selectionID = selectBestChoiceForPlayer(1, randomPlayer);
                            }
                            else
                            {
                                int randomPlayer = team2Players[RNG.roll(team2Players.Count)].ID;
                                selectionID = selectBestChoiceForPlayer(1, randomPlayer);
                            }
                        }
                    }
                    else
                    {
                        selectionID = selectBestOverallChoice(1);
                    }
                    break;
                case 1:     //Best overall hero
                    selectionID = selectBestOverallChoice(1);
                    break;
                case 2:     //Best hero for a player
                    if (team1SelectionPlayerTarget == -1)
                    {
                        team1SelectionPlayerTarget = team1Players[RNG.roll(team1Players.Count)].ID;
                    }
                    selectionID = selectBestChoiceForPlayer(1, team1SelectionPlayerTarget);
                    break;
                case 3:     //Worst overall choice
                    selectionID = selectWorstOverallChoice(1);
                    break;
                case 4:     //Best counter to current team
                    selectionID = selectBestCounterToTeam(2);
                    break;
            }
            return selectionID;
        }

        /// <summary>
        /// Resolves a picking selection for Team 2. Note that this does not actually select anything, but instead returns what the team thinks their best pick is.
        /// </summary>
        public int team2Pick()
        {
            Dictionary<int, int> start = baselineHeroSelection(2);
            int selectionID = -1;
            switch (team2SelectionMode)
            {
                case -1:    //Random selection done due to time running out.
                    selectionID = remainingHeroes[RNG.roll(remainingHeroes.Count)];
                    break;
                case 0:     //Random method of selection.
                    if (RNG.roll(2) == 1)
                    {
                        if (RNG.roll(2) == 1)
                        {
                            if (RNG.roll(4) == 1)
                            {
                                selectionID = selectWorstOverallChoice(2);
                            }
                            else
                            {
                                selectionID = selectBestCounterToTeam(1);
                            }
                        }
                        else
                        {
                            if (RNG.roll(2) == 1)
                            {
                                int randomPlayer = team1Players[RNG.roll(team1Players.Count)].ID;
                                selectionID = selectBestChoiceForPlayer(1, randomPlayer);
                            }
                            else
                            {
                                int randomPlayer = team2Players[RNG.roll(team2Players.Count)].ID;
                                selectionID = selectBestChoiceForPlayer(1, randomPlayer);
                            }
                        }
                    }
                    else
                    {
                        selectionID = selectBestOverallChoice(2);
                    }
                    break;
                case 1:     //Best overall hero
                    selectionID = selectBestOverallChoice(2);
                    break;
                case 2:     //Best hero for a player
                    if (team1SelectionPlayerTarget == -1)
                    {
                        team2SelectionPlayerTarget = team2Players[RNG.roll(team2Players.Count)].ID;
                    }
                    selectionID = selectBestChoiceForPlayer(2, team2SelectionPlayerTarget);
                    break;
                case 3:     //Worst overall choice
                    selectionID = selectWorstOverallChoice(2);
                    break;
                case 4:     //Best counter to current team
                    selectionID = selectBestCounterToTeam(1);
                    break;
            }
            return selectionID;
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Constructs a new MatchSelector.
        /// </summary>
        /// <param name="heroes">The list of all heroes.</param>
        /// <param name="teamA">The first team participating.</param>
        /// <param name="teamB">The second team participating.</param>
        public MatchAI(Match m, Dictionary<int, Hero> heroes, List<Player> teamA, List<Player> teamB)
        {
            match = m;

            allHeroes = heroes;
            team1Players = teamA;
            team2Players = teamB;

            remainingHeroes = allHeroes.Select(kvp => kvp.Key).ToList();
            team1Picks = new List<int>();
            team2Picks = new List<int>();
            team1Bans = new List<int>();
            team2Bans = new List<int>();
            team1Lineup = new List<Tuple<Player, Hero>>();
            team2Lineup = new List<Tuple<Player, Hero>>();
        }
        #endregion

        #region Disposal methods
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                pregameTimer.Dispose();
                tickTimer.Dispose();
            }
        }

        ~MatchAI()
        {
            Dispose(false);
        }
        #endregion
    }
}
