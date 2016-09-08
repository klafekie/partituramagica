using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace PartituraMagica
{
    /** 
     * This view can be used to navigate arbitrary parts.     
     */

    public partial class PartNavigatorStrip : ToolStrip
    {
        private readonly ToolStripItem _btnFirst;
        private readonly ToolStripItem _btnLast;
        private readonly ToolStripItem _btnNext;
        private readonly ToolStripItem _btnPrevious;

        private readonly List<KeyValuePair<string, ToolStripButton>> _partButtons =
            new List<KeyValuePair<string, ToolStripButton>>();

        private int _lastPartIndex;
        private IPartNavigator _navigator;

        private string _partName;

        private string[] _parts;

        public PartNavigatorStrip()
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

            //this is done before the setter of parts Pattern because we're in the c'tor
            foreach (ToolStripItem b in Items)
            {
                b.ImageScaling = ToolStripItemImageScaling.None;
                b.DisplayStyle = ToolStripItemDisplayStyle.Image;
                b.AutoSize = true;
                //b.AutoSize = false;
            }
        }

        /* should be set to a combination of Modifiers. The resulting keyboard shortcuts
         * for navigating are as follows:
         * First <= Modifiers + Home
         * Previous <= Modifiers + Left
         * Next <= Modifiers + Right
         * Last <= Modifiers + End
         * Jump to part <= Modifiers + digit
         */
        public Keys Modifiers { get; set; }

        public string[] Parts
        {
            get { return _parts; }
            set
            {
                bool changed = _parts == null ^ value == null;
                if (_parts != null && value != null)
                {
                    if (_parts.Length != value.Length)
                    {
                        changed = true;
                    }
                    else
                    {
                        for (int i = 0; i < _parts.Length; i++)
                        {
                            if (_parts[i] != value[i])
                            {
                                changed = true;
                            }
                        }
                    }
                }
                if (changed)
                {
                    _parts = value;
                    Adjust(changed);
                }
            }
        }

        private void AdjustIfNecessary(int partIndex)
        {
            bool changed = _lastPartIndex != partIndex;
            _lastPartIndex = partIndex;
            if (changed)
            {
                Adjust(false);
            }
        }


        public event Action<int> PartChanged;

        public event Action<int> PartClicked;

        public event CancellableEvent PreviousPartClicked;

        public event CancellableEvent NextPartClicked;

        private void Adjust(bool partsChanged)
        {
            if (partsChanged)
            {
                //we dont remove the separators if parts is empty.
                foreach (KeyValuePair<string, ToolStripButton> entry in _partButtons)
                {
                    Items.Remove(entry.Value);
                }
                _partButtons.Clear();

                int insertIndex = 3;

                if (_parts != null)
                {
                    for (int i = _parts.Length - 1; i >= 0; i--)
                    {
                        ToolStripButton b = new ToolStripButton(_parts[i]);
                        b.Click += b_Click;
                        //b.AutoSize = false;
                        //b.Width = 36;
                        //b.Height = 36;
                        _partButtons.Add(new KeyValuePair<string, ToolStripButton>(
                            _parts[i], b));
                        Items.Insert(insertIndex, b);
                    }
                    _partButtons.Reverse();
                }
            }
            foreach (KeyValuePair<string, ToolStripButton> entry in _partButtons)
            {
                entry.Value.Checked = false;
            }

            if (_partButtons.Count > _lastPartIndex)
            {
                _partButtons[_lastPartIndex].Value.Checked = true;
            }

            foreach (KeyValuePair<string, ToolStripButton> entry in _partButtons)
            {
                entry.Value.ToolTipText =
                    string.Format("Go to {0} {1}", _partName, entry.Value.Text);
            }
            _btnFirst.ToolTipText =
                string.Format("Go to first {0}", _partName);
            _btnNext.ToolTipText =
                string.Format("Go to next {0}", _partName);
            _btnPrevious.ToolTipText =
                string.Format("Go to previous {0}", _partName);
            _btnLast.ToolTipText =
                string.Format("Go to last {0}", _partName);

            OnPartChanged();
        }

        private void btnLast_Click(object sender, EventArgs e)
        {
            _navigator?.LastPartClicked();
            AdjustIfNecessary(_navigator.CurrentPartIndex);
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            _navigator?.NextPartClicked();
            AdjustIfNecessary(_navigator.CurrentPartIndex);
        }

        private void btnPrevious_Click(object sender, EventArgs e)
        {
            _navigator?.PreviousPartClicked();
            AdjustIfNecessary(_navigator.CurrentPartIndex);
        }

        private void btnFirst_Click(object sender, EventArgs e)
        {
            _navigator?.FirstPartClicked();
            AdjustIfNecessary(_navigator.CurrentPartIndex);
        }

        private void b_Click(object sender, EventArgs e)
        {
            ToolStripButton b = (ToolStripButton) sender;
            _navigator.PartClicked(_partButtons.FindIndex(entry => entry.Value == b));
            AdjustIfNecessary(_navigator.CurrentPartIndex);
        }

        protected virtual void OnPartChanged()
        {
            if (PartChanged != null)
            {
                PartChanged(_lastPartIndex);
            }
        }

        protected virtual void OnPartClicked()
        {
            if (PartClicked != null)
            {
                PartClicked(_lastPartIndex);
            }
        }

        protected virtual bool OnPreviousPartClicked()
        {
            bool cancel = false;
            if (PreviousPartClicked != null)
            {
                PreviousPartClicked(ref cancel);
            }
            return cancel;
        }

        protected virtual bool OnNextPartClicked()
        {
            bool cancel = false;
            if (NextPartClicked != null)
            {
                NextPartClicked(ref cancel);
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
                        JumpToPart(e.KeyCode);
                        break;
                }
            }
        }

        private void JumpToPart(Keys key)
        {
            //key is one based
            //the 0 key will do nothing
            int index = key - Keys.D0 - 1;
            if (index < 0 || index >= _partButtons.Count)
            {
                index = key - Keys.NumPad0 - 1;
            }
            if (index < 0 || index >= _partButtons.Count)
            {
                return;
            }
            _partButtons[index].Value.PerformClick();
        }

        public void RegisterEvents(IPartNavigator navigator)
        {
            _navigator = navigator;
        }
    }
}