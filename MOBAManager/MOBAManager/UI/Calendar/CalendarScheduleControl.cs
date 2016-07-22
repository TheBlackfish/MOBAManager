using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MOBAManager.Management.Teams;
using MOBAManager.Management.Calendar;
using MOBAManager.Utility;

namespace MOBAManager.UI.Calendar
{
    public partial class CalendarScheduleControl : UserControl
    {
        private CalendarManager cm;
        private TeamManager tm;
        private int dayOffset;
        private List<CalendarEvent> possibleEvents;
        private Action<CalendarEvent> submissionFunction;

        private void createAllPossibleEvents()
        {
            //Get all possible PUGs from cm and create events for them.
            foreach (Team t in tm.getAllTeams())
            {
                if (t.ID != 0)
                {
                    if (!cm.teamHasEventsOnDate(t.ID, dayOffset))
                    {
                        if (RNG.coinflip())
                        {
                            possibleEvents.Add(new CalendarEvent(CalendarEvent.EventType.PUG, dayOffset, 0, t.ID));
                        }
                        else
                        {
                            possibleEvents.Add(new CalendarEvent(CalendarEvent.EventType.PUG, dayOffset, t.ID, 0));
                        }
                    }
                }
            }
        }

        private void createAllButtons()
        {
            int curButtonOffset = 0;

            for (int i = 0; i < possibleEvents.Count; i++)
            {
                CalendarEvent cur = possibleEvents[i];

                string desc = "?";
                if (cur.type == CalendarEvent.EventType.PUG)
                {
                    if (cur.team1ID == 0)
                    {
                        desc = "Schedule a pick-up game with " + tm.getTeamName(cur.team2ID);
                    }
                    else
                    {
                        desc = "Schedule a pick-up game with " + tm.getTeamName(cur.team1ID);
                    }
                }

                Button btn = new Button();
                btn.Text = desc;
                btn.Location = new Point(0, curButtonOffset);
                btn.MaximumSize = new Size(buttonContainer.ClientSize.Width - 6, 0);
                btn.MinimumSize = new Size(buttonContainer.ClientSize.Width - 6, 40);
                btn.Margin = new Padding(3);

                //Add button functionality
                int eventIndex = i;
                Action<object, EventArgs> action = (object sender, EventArgs e) =>
                {
                    submissionFunction(possibleEvents[eventIndex]);
                };
                btn.Click += new EventHandler(action);

                buttonContainer.Controls.Add(btn);
                curButtonOffset += btn.Size.Height;
            }
        }

        public CalendarScheduleControl(CalendarManager cm, TeamManager tm, int offset, Action<CalendarEvent> submit)
        {
            InitializeComponent();
            this.cm = cm;
            this.tm = tm;
            dayOffset = offset;
            submissionFunction = submit;
            possibleEvents = new List<CalendarEvent>();
        }

        private void CalendarScheduleControl_ParentChanged(object sender, EventArgs e)
        {
            if (Parent != null)
            {
                Location = new Point((Parent.ClientSize.Width - Size.Width) / 2, (Parent.ClientSize.Height - Size.Height) / 2);
                createAllPossibleEvents();
                createAllButtons();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Parent.Controls.Remove(this);
        }
    }
}
