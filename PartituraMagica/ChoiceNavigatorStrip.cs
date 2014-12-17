using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace PartituraMagica
{
	/** 
	 * This view can be used to navigate arbitrary choices.
	 * In our case Parts or (later) Pages
	 */
	public partial class ChoiceNavigatorStrip : ToolStrip, IChoiceNavigator
	{
		private ToolStripItem btnFirst;
		private ToolStripItem btnLast;
		private ToolStripItem btnNext;
		private ToolStripItem btnPrevious;

		private string choiceName;

		private string[] choices;
		private int current = 0;
		private Keys modifiers;
		private List<KeyValuePair<string, ToolStripButton>> choiceButtons = new List<KeyValuePair<string, ToolStripButton>>();

		public ChoiceNavigatorStrip() {
			InitializeComponent();
			this.ImageList = this.imageList1;//ImageList property cannot be set in designer.						
			//@todo p1 impl Bookmark feature

			
			btnFirst = this.Items.Add("First", this.ImageList.Images["first"]);
			btnFirst.Click += new EventHandler(btnFirst_Click);
			btnPrevious = this.Items.Add("Previous", this.ImageList.Images["previous"]);
			btnPrevious.Click += new EventHandler(btnPrevious_Click);

			this.Items.Add(new ToolStripSeparator());
			this.Items.Add(new ToolStripSeparator());

			btnNext = this.Items.Add("Next", this.ImageList.Images["next"]);
			btnNext.Click += new EventHandler(btnNext_Click);
			btnLast = this.Items.Add("Last", this.ImageList.Images["last"]);
			btnLast.Click += new EventHandler(btnLast_Click);

			//this is done before the setter of choices Pattern because we're in the c'tor
			foreach (ToolStripItem b in this.Items) {
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
		public string ChoiceName {
			get { return choiceName; }
			set {
				choiceName = value;
				Adjust(false);
			}
		}
				
		public string[] Choices {
			get { return choices; }
			set {
				bool changed = choices == null ^ value == null;
				if (choices != null && value != null) {
					if (choices.Length != value.Length) {
						changed = true;
					} else {
						for (int i = 0; i < choices.Length; i++) {
							if (choices[i] != value[i]) {
								changed = true;
							}
						}
					}
				}
				if (changed) {
					choices = value;
					Adjust(changed);
				}
			}
		}

		public int Current {
			get {
				return current;
			}
			set {
				bool changed = current != value;
				current = value;
				if (changed) {
					Adjust(false);
				}
			}
		}

		private void Adjust(bool choicesChanged) {
			if (choicesChanged) {
				//we dont remove the separators if choices is empty.
				foreach (KeyValuePair<string, ToolStripButton> entry in choiceButtons) {
					this.Items.Remove(entry.Value);
				}
				choiceButtons.Clear();

				int insertIndex = 3;

				if (choices != null) {
					for (int i = choices.Length - 1; i >= 0; i--) {
						ToolStripButton b = new ToolStripButton(choices[i].ToString());
						b.Click += new EventHandler(b_Click);
						//b.AutoSize = false;
						//b.Width = 36;
						//b.Height = 36;
						choiceButtons.Add(new KeyValuePair<string, ToolStripButton>(
							choices[i], b));
						this.Items.Insert(insertIndex, b);
					}
					choiceButtons.Reverse();
				}
			}
			foreach (KeyValuePair<string, ToolStripButton> entry in this.choiceButtons) {
				entry.Value.Checked = false;
			}

			if (this.choiceButtons.Count > current) {
				this.choiceButtons[current].Value.Checked = true;
			}

			foreach (KeyValuePair<string, ToolStripButton> entry in this.choiceButtons) {
				entry.Value.ToolTipText =
					String.Format("Go to {0} {1}", choiceName, entry.Value.Text);
			}
			btnFirst.ToolTipText =
				String.Format("Go to first {0}", choiceName);
			btnNext.ToolTipText =
				String.Format("Go to next {0}", choiceName);
			btnPrevious.ToolTipText =
				String.Format("Go to previous {0}", choiceName);
			btnLast.ToolTipText =
				String.Format("Go to last {0}", choiceName);

			OnChoiceChanged();
		}

		/* should be set to a combination of Modifiers. The resulting keyboard shortcuts
		 * for navigating are as follows:
		 * First <= Modifiers + Home
		 * Previous <= Modifiers + Left
		 * Next <= Modifiers + Right
		 * Last <= Modifiers + End
		 * Jump to choice <= Modifiers + digit
		 */
		//@todo p3 this does not allow to jump to choice 10, 11 etc
		//use a timer?
		public Keys Modifiers {
			get { return this.modifiers; }
			set {
				//Debug.Assert((value & Keys.Modifiers) == value);
				this.modifiers = value;
			}
		}

		void btnLast_Click(object sender, EventArgs e) {
			if (this.current < this.choices.Length - 1) {
				this.Current = this.choices.Length - 1;
			}
			OnChoiceClicked();
		}

		void btnNext_Click(object sender, EventArgs e) {
			bool cancel = OnNextChoiceClicked();
			if (!cancel && this.current < this.choices.Length - 1) {
				this.Current++;
			}
			OnChoiceClicked();
		}

		void btnPrevious_Click(object sender, EventArgs e) {
			bool cancel = OnPreviousChoiceClicked();
			if (!cancel && this.current > 0) {
				this.Current--;
			}
			OnChoiceClicked();
		}

		void btnFirst_Click(object sender, EventArgs e) {
			this.Current = 0;
			OnChoiceClicked();
		}

		void b_Click(object sender, EventArgs e) {
			ToolStripButton b = (ToolStripButton)sender;
			this.Current = this.choiceButtons.FindIndex(entry => entry.Value == b);
			OnChoiceClicked();
		}

		public event Action<int> ChoiceChanged;
		protected virtual void OnChoiceChanged() {
			if (ChoiceChanged != null) {
				ChoiceChanged(this.current);
			}
		}

		public event Action<int> ChoiceClicked;
		protected virtual void OnChoiceClicked() {
			if (ChoiceClicked != null) {
				ChoiceClicked(this.current);
			}
		}

		public event CancellableEvent PreviousChoiceClicked;
		protected virtual bool OnPreviousChoiceClicked() {
			bool cancel = false;
			if (PreviousChoiceClicked != null) {
				PreviousChoiceClicked(ref cancel);
			}
			return cancel;
		}

		public event CancellableEvent NextChoiceClicked;
		protected virtual bool OnNextChoiceClicked() {
			bool cancel = false;
			if (NextChoiceClicked != null) {
				NextChoiceClicked(ref cancel);
			}
			return cancel;
		}

		public void HandleKeyDown(object sender, KeyEventArgs e) {
			if (e.Modifiers == this.Modifiers) {
				switch (e.KeyCode) {
					case Keys.Left: btnPrevious.PerformClick(); break;
					case Keys.Right: btnNext.PerformClick(); break;
					case Keys.Home: btnFirst.PerformClick(); break;
					case Keys.End: btnLast.PerformClick(); break;
					default:
						JumpToChoice(e.KeyCode); break;


				}
			}
		}

		private void JumpToChoice(Keys key) {
			//key is one based
			//the 0 key will do nothing
			int index = (int)(key - Keys.D0) - 1;
			if (index < 0 || index >= this.choiceButtons.Count) {
				index = (int)(key - Keys.NumPad0) - 1;
			}
			if (index < 0 || index >= this.choiceButtons.Count) {
				return;
			}
			this.choiceButtons[index].Value.PerformClick();
		}
	}
}
