﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MOBAManager.Management.Calendar;
using MOBAManager.Management.Teams;

namespace MOBAManager.UI.Calendar
{
    public partial class CalendarViewControl : UserControl
    {
        #region Private variables
        /// <summary>
        /// The calendar manager of the control.
        /// </summary>
        private CalendarManager cm;

        /// <summary>
        /// The DateTime corresponding to the first day of the current month.
        /// </summary>
        private DateTime baseline;

        /// <summary>
        /// The dictionary containing all of the month grids generated thus far, stored by string in the form of "MM-YYYY".
        /// </summary>
        private Dictionary<string, TableLayoutPanel> monthGrids;

        /// <summary>
        /// The current offset from 
        /// </summary>
        private int currentMonthOffset = 0;

        /// <summary>
        /// The team manager of the current game.
        /// </summary>
        private TeamManager tm;
        #endregion

        #region Public methods
        public void submitCalenderEvent(CalendarEvent submit)
        {
            cm.addCalendarEvent(submit);
            foreach (Control c in Controls)
            {
                if (c is CalendarScheduleControl)
                {
                    Controls.Remove(c);
                }
            }
        }
        #endregion

        #region Private methods
        /// <summary>
        /// Figures out which month to show and then either a) retrieves its grid from the dictionary of grids, or b) constructs a new grid for that month. Either way,
        /// the grid is then shown on-screen.
        /// </summary>
        private void updateCalendarContainer()
        {
            int monthToShow = baseline.Month + currentMonthOffset;
            int yearToShow = baseline.Year;
            while (monthToShow > 12)
            {
                monthToShow -= 12;
                yearToShow++;
            }

            DateTime toShow = baseline.AddMonths(currentMonthOffset);
            monthLabel.Text = toShow.ToString("MMMM yyyy");

            TableLayoutPanel newMonth = getMonthlyPanel(toShow.Month, toShow.Year);
            if (!calendarContainer.Controls.Contains(newMonth))
            {
                calendarContainer.Controls.Add(newMonth);
            }

            foreach (Control c in calendarContainer.Controls)
            {
                if (c is TableLayoutPanel)
                {
                    c.Hide();
                }
            }
            newMonth.Show();
            newMonth.Location = new Point(0, 0 - calendarContainer.AutoScrollPosition.Y);
            newMonth.BringToFront();
        }

        /// <summary>
        /// This either returns a previously created grid for the specified month and year, or creates a new one.
        /// </summary>
        /// <param name="month">The month to get a grid for.</param>
        /// <param name="year">The year to get a grid for.</param>
        /// <returns></returns>
        private TableLayoutPanel getMonthlyPanel(int month, int year)
        {
            string key = month + "-" + year;
            if (monthGrids.ContainsKey(key))
            {
                return monthGrids[key];
            }
            else
            {
                List<Tuple<int, string>> playerEvents = cm.getEventStatusForTeamInMonth(0, month, year);
                int baseDayOffset = cm.getDaysToDate(new DateTime(year, month, 1));

                TableLayoutPanel monthGrid = new TableLayoutPanel();
                monthGrid.Location = new System.Drawing.Point(3, 3);
                monthGrid.Name = month + "-" + year + "Grid";
                monthGrid.Size = new Size(calendarContainer.ClientSize.Width - 20, calendarContainer.Size.Height - 6);
                monthGrid.RowCount = DateTime.DaysInMonth(year, month);

                int totalHeight = 0;
                for (int i = 1; i <= DateTime.DaysInMonth(year, month); i++)
                {
                    Label dateLabel = new Label();
                    dateLabel.Font = new Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                    dateLabel.TextAlign = ContentAlignment.MiddleCenter;
                    dateLabel.Text = i.ToString();
                    dateLabel.Size = new Size(48, 48);
                    dateLabel.Margin = new Padding(0);

                    //Get current situation about date for player team
                    Label explanationText = new Label();
                    explanationText.Font = new Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                    explanationText.TextAlign = ContentAlignment.MiddleCenter;
                    explanationText.AutoSize = true;
                    explanationText.MaximumSize = new Size(monthGrid.Size.Width - 96, 0);
                    explanationText.MinimumSize = new Size(monthGrid.Size.Width - 96, 48);
                    explanationText.Text = playerEvents[i - 1].Item2;
                    explanationText.Margin = new Padding(0);

                    //Add button to end if nothing scheduled
                    Button affectDate = new Button();
                    affectDate.Text = "!";
                    affectDate.Size = new Size(48, 48);
                    affectDate.Margin = new Padding(0);

                    int affectOffset = baseDayOffset + i - 1;
                    Action<object, EventArgs> action = (object sender, EventArgs e) =>
                    {
                        showScheduleControl(affectOffset);
                    };
                    affectDate.Click += new EventHandler(action);


                    if (playerEvents[i - 1].Item1 == -1)
                    {
                        dateLabel.ForeColor = Color.Gray;
                        explanationText.ForeColor = Color.Gray;
                        affectDate.Enabled = false;
                    }

                    monthGrid.Controls.Add(dateLabel, 0, i - 1);
                    monthGrid.Controls.Add(explanationText, 1, i - 1);
                    monthGrid.Controls.Add(affectDate, 2, i - 1);

                    totalHeight += explanationText.Height;
                }

                monthGrid.Size = new Size(monthGrid.Size.Width, totalHeight);

                monthGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 48));
                monthGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, monthGrid.Size.Width - 96));
                monthGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 48));

                monthGrids.Add(key, monthGrid);
                return monthGrid;
            }
        }

        private void showScheduleControl(int offset)
        {
            CalendarScheduleControl csc = new CalendarScheduleControl(cm, tm, offset);
            Controls.Add(csc);
            csc.BringToFront();
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new calendar control.
        /// </summary>
        /// <param name="cm">The calendar manager of the current game.</param>
        /// <param name="tm">The team manager of the current game.</param>
        public CalendarViewControl(CalendarManager cm, TeamManager tm)
        {
            InitializeComponent();
            this.cm = cm;
            this.tm = tm;
            baseline = cm.getFormattedDateTime();

            monthGrids = new Dictionary<string, TableLayoutPanel>();

            updateCalendarContainer();
        }
        #endregion

        #region Events
        /// <summary>
        /// Called when the calendar control's parent changes. Resizes the control to fits its parent.
        /// </summary>
        private void CalendarViewControl_ParentChanged(object sender, EventArgs e)
        {
            if (Parent != null)
            {
                Size = Parent.ClientSize;
            }
        }

        /// <summary>
        /// Called when the calendar control resizes. Resizes all table grids to fit the center panel better due to absolute sizing.
        /// </summary>
        private void calendarContainer_Resize(object sender, EventArgs e)
        {
            foreach(TableLayoutPanel tlp in monthGrids.Values)
            {
                tlp.Size = new Size(calendarContainer.ClientSize.Width - 20, tlp.Size.Height);
                tlp.ColumnStyles[1] = new ColumnStyle(SizeType.Absolute, tlp.Width - 96);
                for (int i = 0; i < tlp.RowCount - 1; i++)
                {
                    tlp.GetControlFromPosition(1, i).MaximumSize = new Size(tlp.Size.Width - 96, 0);
                    tlp.GetControlFromPosition(1, i).MinimumSize = new Size(tlp.Size.Width - 96, 48);
                }
            }
        }

        /// <summary>
        /// Called when the PREV button is clicked. Decrements the month offset.
        /// </summary>
        private void prevMonthButton_Click(object sender, EventArgs e)
        {
            currentMonthOffset--;
            if (currentMonthOffset < 0)
            {
                currentMonthOffset = 0;
            }
            else
            {
                updateCalendarContainer();
            }
        }

        /// <summary>
        /// Called when the NEXT button is click. Increments the month offset.
        /// </summary>
        private void nextMonthButton_Click(object sender, EventArgs e)
        {
            currentMonthOffset++;
            if (currentMonthOffset > 12)
            {
                currentMonthOffset = 12;
            }
            else
            {
                updateCalendarContainer();
            }
        }
        #endregion
    }
}
