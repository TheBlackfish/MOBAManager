using MOBAManager.Management.Players;
using MOBAManager.Management.Teams;
using MOBAManager.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOBAManager.Resolution.BootcampResolution
{
    //Item 2 - 0 = Hero Training
    //         1 = Teamwork Training
    //         2 = Whatever training
    sealed public class BootcampSession
    {
        #region Private variables
        /// <summary>
        /// The name of the team involved with this bootcamp.
        /// </summary>
        public string teamName;

        /// <summary>
        /// Control variable for if the bootcamp session is player-controlled or not.
        /// </summary>
        public bool isPlayerControlled;

        /// <summary>
        /// The list of 'training modes' for each player in this bootcamp.
        /// <para>The first entry is the Player object for the player, followed by the mode of training and the training mode specifier.</para>
        /// <para>The modes are:</para>
        /// <para>0 - Hero Training. The specifying entry is the ID of the hero to train with.</para>
        /// <para>1 - Teamwork Training. The player gains teamwork skill with all other players in the bootcamp.</para>
        /// <para>2 - Pure Training. The player attempts to gain pure skill.</para>
        /// </summary>
        private List<Tuple<Player, int, int>> trainingModes;

        /// <summary>
        /// The ordered list of hero names with their IDs.
        /// </summary>
        private List<Tuple<int, string>> heroNames;
        #endregion

        #region Private methods
        /// <summary>
        /// Fully resolves the training by increasing each player's skill levels based on the training modes selected.
        /// </summary>
        /// <returns></returns>
        private bool ResolveTraining()
        {
            foreach (Tuple<Player, int, int> t in trainingModes)
            {
                if (t.Item2 == -1)
                {
                    return false;
                }
                else if (t.Item2 == 0 && t.Item3 == -1)
                {
                    return false;
                }
            }
            
            foreach (Tuple<Player, int, int> t in trainingModes)
            {
                switch (t.Item2)
                {
                    case 0:
                        t.Item1.GainHeroExperience(t.Item3);
                        t.Item1.GainHeroExperience(t.Item3);
                        break;
                    case 2:
                        t.Item1.GainPSExperience();
                        break;
                    case 1:
                        foreach (int id in trainingModes.Where(tm => tm.Item1.ID != t.Item1.ID).Select(tm => tm.Item1.ID).ToList())
                        {
                            t.Item1.GainTeamworkExperience(id);
                        }
                        break;
                }
            }

            return true;
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Returns a list of all of the names of all players participating in this bootcamp.
        /// </summary>
        /// <returns></returns>
        public List<string> GetPlayerNames()
        {
            return (trainingModes.Select(t => t.Item1.PlayerName).ToList());
        }

        /// <summary>
        /// Returns a list of all of the names of all heroes.
        /// </summary>
        /// <returns></returns>
        public List<string> GetHeroNames()
        {
            return (heroNames.OrderBy(t => t.Item1).Select(t => t.Item2).ToList());
        }

        /// <summary>
        /// Inserts a training mode for a player at the specified index.
        /// </summary>
        /// <param name="index">The index of the player to submit the training for.</param>
        /// <param name="mode">The type of training for this player. If 0, will return false as the 0 training mode requires a specification.</param>
        /// <returns></returns>
        public bool SubmitTrainingMode(int index, int mode)
        {
            if (mode == 0)
            {
                return false;
            }
            return SubmitTrainingMode(index, mode, -1);
        }

        /// <summary>
        /// Inserts a training mode for a player at the specified index.
        /// </summary>
        /// <param name="index">The index of the player to submit the training for.</param>
        /// <param name="mode">The type of training for this player.</param>
        /// <param name="specific">The specific parameter for training. Only needed if the training mode is 0.</param>
        /// <returns></returns>
        public bool SubmitTrainingMode(int index, int mode, int specific)
        {
            Tuple<Player, int, int> prevTM = trainingModes[index];
            Tuple<Player, int, int> tm = new Tuple<Player, int, int>(prevTM.Item1, mode, specific);
            trainingModes[index] = tm;
            return true;
        }

        /// <summary>
        /// Gets a brief description of the bootcamp for the event resolution screen.
        /// </summary>
        /// <returns></returns>
        public string GetSummary()
        {
            return teamName + " has completed their bootcamp.";
        }

        /// <summary>
        /// Instantly resolves the bootcamp by selecting random training modes for each player.
        /// </summary>
        public void InstantlyResolve()
        {
            List<Tuple<Player, int, int>> temp = new List<Tuple<Player, int, int>>();
            foreach (Tuple<Player, int, int> t in trainingModes)
            {
                int mode = RNG.Roll(3);
                int heroID = -1;
                if (mode == 0)
                {
                    heroID = RNG.Roll(heroNames.Count);
                }
                temp.Add(new Tuple<Player, int, int>(t.Item1, mode, heroID));
            }
            trainingModes = temp;
            ResolveTraining();
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new bootcamp.
        /// </summary>
        /// <param name="team">The team that is holding the bootcamp.</param>
        /// <param name="heroNames">The list of ordered hero names with IDs for reference.</param>
        public BootcampSession(Team team, List<Tuple<int, string>> heroNames)
        {
            teamName = team.TeamName;
            trainingModes = new List<Tuple<Player, int, int>>();
            foreach(Player p in team.GetTeammates())
            {
                trainingModes.Add(new Tuple<Player, int, int>(p, -1, -1));
            }
            this.heroNames = heroNames;
        }

        /// <summary>
        /// Creates a player-controlled bootcamp.
        /// </summary>
        /// <param name="team">The team that is holding the bootcamp.</param>
        /// <param name="heroNames">The list of ordered hero names with IDs for reference.</param>
        /// <param name="isPlayerControlled">If true, this bootcamp will require player </param>
        public BootcampSession(Team team, List<Tuple<int, string>> heroNames, bool isPlayerControlled)
            : this(team, heroNames)
        {
            this.isPlayerControlled = isPlayerControlled;
        }
        #endregion
    }
}
