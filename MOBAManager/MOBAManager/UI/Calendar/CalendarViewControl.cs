using System;
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
        private CalendarManager cm;
        private TeamManager tm;
        private DateTime baseline;
        private int currentMonthOffset = 0;
        private Dictionary<string, TableLayoutPanel> monthGrids;

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

        private TableLayoutPanel getMonthlyPanel(int month, int year)
        {
            string key = month + "-" + year;
            if (monthGrids.ContainsKey(key))
            {
                return monthGrids[key];
            }
            else
            {
                List<Tuple<int, string>> playerEvents = cm.getEventStatusForTeamInMonth(0, month, year, tm);

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

        public CalendarViewControl(CalendarManager cm, TeamManager tm)
        {
            InitializeComponent();
            this.cm = cm;
            this.tm = tm;
            baseline = cm.getFormattedDateTime();

            monthGrids = new Dictionary<string, TableLayoutPanel>();

            updateCalendarContainer();
        }

        private void CalendarViewControl_ParentChanged(object sender, EventArgs e)
        {
            if (Parent != null)
            {
                Size = Parent.Size;
            }
        }

        private void calendarContainer_Resize(object sender, EventArgs e)
        {
            foreach(TableLayoutPanel tlp in monthGrids.Values)
            {
                tlp.Size = new Size(calendarContainer.ClientSize.Width - 20, tlp.Size.Height);
                tlp.ColumnStyles[1] = new ColumnStyle(SizeType.Absolute, tlp.Width - 96);
                for (int i = 0; i < tlp.RowCount - 1; i++)
                {
                    Control temp = tlp.GetControlFromPosition(1, i);
                    temp.Size = new Size(tlp.Size.Width - 96, 48);
                }
            }
        }

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
    }
}
