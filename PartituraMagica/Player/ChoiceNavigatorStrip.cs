using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace PartituraMagica
{
    /** 
     * This view can be used to navigate arbitrary choices.
     * In our case Parts or (later) Pages
     */

    public partial class ChoiceNavigatorStrip : ToolStrip, IChoiceNavigator
    {
        private readonly ToolStripItem btnFirst;
        private readonly ToolStripItem btnLast;
        private readonly ToolStripItem btnNext;
        private readonly ToolStripItem btnPrevious;

        private readonly List<KeyValuePair<string, ToolStripButton>> choiceButtons =
            new List<KeyValuePair<string, ToolStripButton>>();

        private string choiceName;

        private string[] choices;
        private int current;

        public ChoiceNavigatorStrip()
        {
            InitializeComponent();
            ImageList = imageList1; //ImageList property cannot be set in designer.						
            //@todo p1 impl Bookmark feature


            btnFirst = Items.Add("First", ImageList.Images["first"]);
            btnFirst.Click += btnFirst_Click;
            btnPrevious = Items.Add("Previous", ImageList.Images["previous"]);
            btnPrevious.Click += btnPrevious_Click;

            Items.Add(new ToolStripSeparator());
            Items.Add(new ToolStripSeparator());

            btnNext = Items.Add("Next", ImageList.Images["next"]);
            btnNext.Click += btnNext_Click;
            btnLast = Items.Add("Last", ImageList.Images["last"]);
            btnLast.Click += btnLast_Click;

            //this is done before the setter of choices Pattern because we're in the c'tor
            foreach (ToolStripItem b in Items)
            {
                b.ImageScaling = ToolStripItemImageScaling.None;
                b.DisplayStyle = ToolStripItemDisplayStyle.Image;
                b.AutoSize = true;
                //b.AutoSize = false;
            }
        }

        /** 
         * sets the tooltip and button texts to reflect the type of choice
         * you can navigate. E.G. "Part" or "Page"
         */

        public string ChoiceName
        {
            get { return choiceName; }
            set
            {
                choiceName = value;
                Adjust(false);
            }
        }

        /* should be set to a combination of Modifiers. The resulting keyboard shortcuts
         * for navigating are as follows:
         * First <= Modifiers + Home
         * Previous <= Modifiers + Left
         * Next <= Modifiers + Right
         * Last <= Modifiers + End
         * Jump to choice <= Modifiers + digit
         */
        public Keys Modifiers { get; set; }

        public string[] Choices
        {
            get { return choices; }
            set
            {
                bool changed = choices == null ^ value == null;
                if (choices != null && value != null)
                {
                    if (choices.Length != value.Length)
                    {
                        changed = true;
                    }
                    else
                    {
                        for (int i = 0; i < choices.Length; i++)
                        {
                            if (choices[i] != value[i])
                            {
                                changed = true;
                            }
                        }
                    }
                }
                if (changed)
                {
                    choices = value;
                    Adjust(changed);
                }
            }
        }

        public int Current
        {
            get { return current; }
            set
            {
                bool changed = current != value;
                current = value;
                if (changed)
                {
                    Adjust(false);
                }
            }
        }

        public event Action<int> ChoiceChanged;

        public event Action<int> ChoiceClicked;

        public event CancellableEvent PreviousChoiceClicked;

        public event CancellableEvent NextChoiceClicked;

        private void Adjust(bool choicesChanged)
        {
            if (choicesChanged)
            {
                //we dont remove the separators if choices is empty.
                foreach (KeyValuePair<string, ToolStripButton> entry in choiceButtons)
                {
                    Items.Remove(entry.Value);
                }
                choiceButtons.Clear();

                int insertIndex = 3;

                if (choices != null)
                {
                    for (int i = choices.Length - 1; i >= 0; i--)
                    {
                        ToolStripButton b = new ToolStripButton(choices[i]);
                        b.Click += b_Click;
                        //b.AutoSize = false;
                        //b.Width = 36;
                        //b.Height = 36;
                        choiceButtons.Add(new KeyValuePair<string, ToolStripButton>(
                            choices[i], b));
                        Items.Insert(insertIndex, b);
                    }
                    choiceButtons.Reverse();
                }
            }
            foreach (KeyValuePair<string, ToolStripButton> entry in choiceButtons)
            {
                entry.Value.Checked = false;
            }

            if (choiceButtons.Count > current)
            {
                choiceButtons[current].Value.Checked = true;
            }

            foreach (KeyValuePair<string, ToolStripButton> entry in choiceButtons)
            {
                entry.Value.ToolTipText =
                    string.Format("Go to {0} {1}", choiceName, entry.Value.Text);
            }
            btnFirst.ToolTipText =
                string.Format("Go to first {0}", choiceName);
            btnNext.ToolTipText =
                string.Format("Go to next {0}", choiceName);
            btnPrevious.ToolTipText =
                string.Format("Go to previous {0}", choiceName);
            btnLast.ToolTipText =
                string.Format("Go to last {0}", choiceName);

            OnChoiceChanged();
        }

        private void btnLast_Click(object sender, EventArgs e)
        {
            if (current < choices.Length - 1)
            {
                Current = choices.Length - 1;
            }
            OnChoiceClicked();
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            bool cancel = OnNextChoiceClicked();
            if (!cancel && current < choices.Length - 1)
            {
                Current++;
            }
            OnChoiceClicked();
        }

        private void btnPrevious_Click(object sender, EventArgs e)
        {
            bool cancel = OnPreviousChoiceClicked();
            if (!cancel && current > 0)
            {
                Current--;
            }
            OnChoiceClicked();
        }

        private void btnFirst_Click(object sender, EventArgs e)
        {
            Current = 0;
            OnChoiceClicked();
        }

        private void b_Click(object sender, EventArgs e)
        {
            ToolStripButton b = (ToolStripButton) sender;
            Current = choiceButtons.FindIndex(entry => entry.Value == b);
            OnChoiceClicked();
        }

        protected virtual void OnChoiceChanged()
        {
            if (ChoiceChanged != null)
            {
                ChoiceChanged(current);
            }
        }

        protected virtual void OnChoiceClicked()
        {
            if (ChoiceClicked != null)
            {
                ChoiceClicked(current);
            }
        }

        protected virtual bool OnPreviousChoiceClicked()
        {
            bool cancel = false;
            if (PreviousChoiceClicked != null)
            {
                PreviousChoiceClicked(ref cancel);
            }
            return cancel;
        }

        protected virtual bool OnNextChoiceClicked()
        {
            bool cancel = false;
            if (NextChoiceClicked != null)
            {
                NextChoiceClicked(ref cancel);
            }
            return cancel;
        }

        public void HandleKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Modifiers == Modifiers)
            {
                switch (e.KeyCode)
                {
                    case Keys.Left:
                        btnPrevious.PerformClick();
                        break;
                    case Keys.Right:
                        btnNext.PerformClick();
                        break;
                    case Keys.Home:
                        btnFirst.PerformClick();
                        break;
                    case Keys.End:
                        btnLast.PerformClick();
                        break;
                    default:
                        JumpToChoice(e.KeyCode);
                        break;
                }
            }
        }

        private void JumpToChoice(Keys key)
        {
            //key is one based
            //the 0 key will do nothing
            int index = key - Keys.D0 - 1;
            if (index < 0 || index >= choiceButtons.Count)
            {
                index = key - Keys.NumPad0 - 1;
            }
            if (index < 0 || index >= choiceButtons.Count)
            {
                return;
            }
            choiceButtons[index].Value.PerformClick();
        }
    }
}