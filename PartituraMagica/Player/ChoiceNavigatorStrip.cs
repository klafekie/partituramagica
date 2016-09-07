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
        private readonly ToolStripItem _btnFirst;
        private readonly ToolStripItem _btnLast;
        private readonly ToolStripItem _btnNext;
        private readonly ToolStripItem _btnPrevious;

        private readonly List<KeyValuePair<string, ToolStripButton>> _choiceButtons =
            new List<KeyValuePair<string, ToolStripButton>>();

        private string _choiceName;

        private string[] _choices;
        private int _current;

        public ChoiceNavigatorStrip()
        {
            InitializeComponent();
            ImageList = imageList1; //ImageList property cannot be set in designer.						
            //@todo p1 impl Bookmark feature


            _btnFirst = Items.Add("First", ImageList.Images["first"]);
            _btnFirst.Click += btnFirst_Click;
            _btnPrevious = Items.Add("Previous", ImageList.Images["previous"]);
            _btnPrevious.Click += btnPrevious_Click;

            Items.Add(new ToolStripSeparator());
            Items.Add(new ToolStripSeparator());

            _btnNext = Items.Add("Next", ImageList.Images["next"]);
            _btnNext.Click += btnNext_Click;
            _btnLast = Items.Add("Last", ImageList.Images["last"]);
            _btnLast.Click += btnLast_Click;

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
            get { return _choiceName; }
            set
            {
                _choiceName = value;
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
            get { return _choices; }
            set
            {
                bool changed = _choices == null ^ value == null;
                if (_choices != null && value != null)
                {
                    if (_choices.Length != value.Length)
                    {
                        changed = true;
                    }
                    else
                    {
                        for (int i = 0; i < _choices.Length; i++)
                        {
                            if (_choices[i] != value[i])
                            {
                                changed = true;
                            }
                        }
                    }
                }
                if (changed)
                {
                    _choices = value;
                    Adjust(changed);
                }
            }
        }

        public int Current
        {
            get { return _current; }
            set
            {
                bool changed = _current != value;
                _current = value;
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
                foreach (KeyValuePair<string, ToolStripButton> entry in _choiceButtons)
                {
                    Items.Remove(entry.Value);
                }
                _choiceButtons.Clear();

                int insertIndex = 3;

                if (_choices != null)
                {
                    for (int i = _choices.Length - 1; i >= 0; i--)
                    {
                        ToolStripButton b = new ToolStripButton(_choices[i]);
                        b.Click += b_Click;
                        //b.AutoSize = false;
                        //b.Width = 36;
                        //b.Height = 36;
                        _choiceButtons.Add(new KeyValuePair<string, ToolStripButton>(
                            _choices[i], b));
                        Items.Insert(insertIndex, b);
                    }
                    _choiceButtons.Reverse();
                }
            }
            foreach (KeyValuePair<string, ToolStripButton> entry in _choiceButtons)
            {
                entry.Value.Checked = false;
            }

            if (_choiceButtons.Count > _current)
            {
                _choiceButtons[_current].Value.Checked = true;
            }

            foreach (KeyValuePair<string, ToolStripButton> entry in _choiceButtons)
            {
                entry.Value.ToolTipText =
                    string.Format("Go to {0} {1}", _choiceName, entry.Value.Text);
            }
            _btnFirst.ToolTipText =
                string.Format("Go to first {0}", _choiceName);
            _btnNext.ToolTipText =
                string.Format("Go to next {0}", _choiceName);
            _btnPrevious.ToolTipText =
                string.Format("Go to previous {0}", _choiceName);
            _btnLast.ToolTipText =
                string.Format("Go to last {0}", _choiceName);

            OnChoiceChanged();
        }

        private void btnLast_Click(object sender, EventArgs e)
        {
            if (_current < _choices.Length - 1)
            {
                Current = _choices.Length - 1;
            }
            OnChoiceClicked();
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            bool cancel = OnNextChoiceClicked();
            if (!cancel && _current < _choices.Length - 1)
            {
                Current++;
            }
            OnChoiceClicked();
        }

        private void btnPrevious_Click(object sender, EventArgs e)
        {
            bool cancel = OnPreviousChoiceClicked();
            if (!cancel && _current > 0)
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
            Current = _choiceButtons.FindIndex(entry => entry.Value == b);
            OnChoiceClicked();
        }

        protected virtual void OnChoiceChanged()
        {
            if (ChoiceChanged != null)
            {
                ChoiceChanged(_current);
            }
        }

        protected virtual void OnChoiceClicked()
        {
            if (ChoiceClicked != null)
            {
                ChoiceClicked(_current);
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
                        _btnPrevious.PerformClick();
                        break;
                    case Keys.Right:
                        _btnNext.PerformClick();
                        break;
                    case Keys.Home:
                        _btnFirst.PerformClick();
                        break;
                    case Keys.End:
                        _btnLast.PerformClick();
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
            if (index < 0 || index >= _choiceButtons.Count)
            {
                index = key - Keys.NumPad0 - 1;
            }
            if (index < 0 || index >= _choiceButtons.Count)
            {
                return;
            }
            _choiceButtons[index].Value.PerformClick();
        }
    }
}