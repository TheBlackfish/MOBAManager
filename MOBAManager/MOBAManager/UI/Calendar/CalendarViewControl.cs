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
    sealed public partial class CalendarViewControl : UserControl
    {
        #region Private variables
        /// <summary>
        /// The calendar manager of the control.
        /// </summary>
        private readonly CalendarManager cm;

        /// <summary>
        /// The DateTime corresponding to the first day of the current month.
        /// </summary>
        private readonly DateTime baseline;

        /// <summary>
        /// The dictionary containing all of the month grids generated thus far, stored by string in the form of "MM-YYYY".
        /// </summary>
        private readonly Dictionary<string, TableLayoutPanel> monthGrids;

        /// <summary>
        /// The current offset from 
        /// </summary>
        private int currentMonthOffset = 0;

        /// <summary>
        /// The team manager of the current game.
        /// </summary>
        private readonly TeamManager tm;

        /// <summary>
        /// The function to call when the calender control should be closed.
        /// </summary>
        private readonly Action closeFunc = null;

        /// <summary>
        /// The size control to prevent unnecessary recalculations.
        /// </summary>
        private Size currentSize = new Size(0, 0);
        #endregion

        #region Public methods
        /// <summary>
        /// Submits the given event to the calendar manager and updates the current month grid to reflect the new data.
        /// </summary>
        /// <param name="submit"></param>
        public void SubmitCalendarEvent(CalendarEvent submit)
        {
            if (submit != null)
            {
                cm.AddCalendarEvent(submit);
                UpdateCalendarContainer(true);
                foreach (Control c in Controls)
                {
                    if (c is CalendarScheduleControl)
                    {
                        Controls.Remove(c);
                    }
                }
            }
        }
        #endregion

        #region Private methods
        /// <summary>
        /// Prepares monthly grids for each month in the upcoming year.
        /// </summary>
        /// <param name="offset">Recursive monthly offset.</param>
        private void prepareCalendarContainer(int offset)
        {
            if (offset <= 12)
            {
                DateTime toShow = baseline.AddMonths(offset);
                GetMonthlyPanel(toShow.Month, toShow.Year);
                prepareCalendarContainer(offset+1);
            }
        }

        /// <summary>
        /// Figures out which month to show and then either a) retrieves its grid from the dictionary of grids, or b) constructs a new grid for that month. Either way,
        /// the grid is then shown on-screen.
        /// </summary>
        private void UpdateCalendarContainer()
        {
            DateTime toShow = baseline.AddMonths(currentMonthOffset);
            monthLabel.Text = toShow.ToString("MMMM yyyy");

            TableLayoutPanel newMonth = GetMonthlyPanel(toShow.Month, toShow.Year);
            Action update = () =>
            {
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
            };
            BeginInvoke(update);
        }

        /// <summary>
        /// Forces the calendar container to fully update a month's grid.
        /// </summary>
        /// <param name="fullUpdate"></param>
        private void UpdateCalendarContainer(bool fullUpdate)
        {
            if (fullUpdate)
            {
                DateTime toShow = baseline.AddMonths(currentMonthOffset);
                monthLabel.Text = toShow.ToString("MMMM yyyy");

                TableLayoutPanel newMonth = GetMonthlyPanel(toShow.Month, toShow.Year);
                calendarContainer.Controls.Remove(newMonth);
                foreach (KeyValuePair<string, TableLayoutPanel> item in monthGrids.Where(kvp => kvp.Value == newMonth).ToList())
                {
                    monthGrids.Remove(item.Key);
                }
            }
            UpdateCalendarContainer();
        }

        /// <summary>
        /// This either returns a previously created grid for the specified month and year, or creates a new one.
        /// </summary>
        /// <param name="month">The month to get a grid for.</param>
        /// <param name="year">The year to get a grid for.</param>
        /// <returns></returns>
        private TableLayoutPanel GetMonthlyPanel(int month, int year)
        {
            string key = month + "-" + year;
            if (monthGrids.ContainsKey(key))
            {
                return monthGrids[key];
            }
            else
            {
                List<Tuple<int, string>> playerEvents = cm.GetEventStatusForTeamInMonth(0, month, year);
                List<Tuple<int, string>> tournaments = cm.GetTournamentsInMonth(month, year);
                int baseDayOffset = cm.GetDaysToDate(new DateTime(year, month, 1));

                TableLayoutPanel monthGrid = new TableLayoutPanel();
                monthGrid.Location = new Point(3, 3);
                monthGrid.Name = month + "-" + year + "Grid";
                monthGrid.Size = new Size(calendarContainer.Size.Width - 20 - SystemInformation.VerticalScrollBarWidth, calendarContainer.Size.Height - 6);
                monthGrid.RowCount = DateTime.DaysInMonth(year, month);
                monthGrid.SuspendLayout();

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
                    string text = playerEvents[i - 1].Item2;
                    if (text.Length > 0)
                    {
                        if (tournaments[i - 1].Item2.Length > 0)
                        {
                            text += Environment.NewLine + tournaments[i - 1].Item2;
                        }
                    }
                    else
                    {
                        text = tournaments[i - 1].Item2;
                    }
                    if (text.Length == 0)
                    {
                        text = "---";
                    }
                    explanationText.Text = text;
                    explanationText.Margin = new Padding(0);

                    //Add button to end if nothing scheduled
                    Button affectDate = new Button();
                    affectDate.Text = "!";
                    affectDate.Size = new Size(48, 48);
                    affectDate.Margin = new Padding(0);

                    int affectOffset = baseDayOffset + i - 1;
                    Action<object, EventArgs> action = (object sender, EventArgs e) =>
                    {
                        ShowScheduleControl(affectOffset);
                    };
                    affectDate.Click += new EventHandler(action);


                    if (playerEvents[i - 1].Item1 == -1)
                    {
                        dateLabel.ForeColor = Color.Gray;
                        explanationText.ForeColor = Color.Gray;
                        affectDate.Enabled = false;
                    }
                    
                    if (tournaments[i - 1].Item1 == -1)
                    {
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
                monthGrid.ResumeLayout(false);

                if (!monthGrids.ContainsKey(key))
                {
                    monthGrids.Add(key, monthGrid);
                }
                return monthGrid;
            }
        }

        /// <summary>
        /// Creates and shows a scheduling window for the specified offset.
        /// </summary>
        /// <param name="offset"></param>
        private void ShowScheduleControl(int offset)
        {
            CalendarScheduleControl csc = new CalendarScheduleControl(cm, tm, offset, SubmitCalendarEvent);
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
        public CalendarViewControl(CalendarManager cm, TeamManager tm, Action onClose)
        {
            InitializeComponent();
            this.cm = cm;
            this.tm = tm;
            closeFunc = onClose;
            baseline = CalendarManager.GetFormattedDateTime();

            monthGrids = new Dictionary<string, TableLayoutPanel>();

            Task.Run(() =>
            {
                UpdateCalendarContainer();
            });

            Task.Run(() =>
            {
                prepareCalendarContainer(1);
            });
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
                Size = Parent.Size;
            }
        }

        /// <summary>
        /// Called when the calendar control resizes. Resizes all table grids to fit the center panel better due to absolute sizing.
        /// </summary>
        private void calendarContainer_Resize(object sender, EventArgs e)
        {
            if (Size.Width != currentSize.Width && Size.Height != currentSize.Height)
            {
                foreach (TableLayoutPanel tlp in monthGrids.Values)
                {
                    tlp.Size = new Size(calendarContainer.Size.Width - 20 - System.Windows.Forms.SystemInformation.VerticalScrollBarWidth, tlp.Size.Height);
                    tlp.ColumnStyles[1] = new ColumnStyle(SizeType.Absolute, tlp.Size.Width - 96);
                    for (int i = 0; i < tlp.RowCount; i++)
                    {
                        tlp.GetControlFromPosition(1, i).MaximumSize = new Size(tlp.Size.Width - 96, 0);
                        tlp.GetControlFromPosition(1, i).MinimumSize = new Size(tlp.Size.Width - 96, 48);
                    }
                }
                currentSize = Size;
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
                UpdateCalendarContainer();
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
                UpdateCalendarContainer();
            }
        }
        
        /// <summary>
        /// Calls the close function of the calender view.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void returnButton_Click(object sender, EventArgs e)
        {
            if (closeFunc != null)
            {
                closeFunc.Invoke();
            }
        }
        #endregion
    }
}
