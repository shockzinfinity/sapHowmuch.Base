using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace sapHowmuch.Base.TestWinformInstaller.Misc
{
	public class ImageButton : PictureBox, IButtonControl
	{
		private const int WM_KEYDOWN = 0x0100;
		private const int WM_KEYUP = 0x0101;
		private bool _hover = false;
		private bool _down = false;
		private bool _isDefault = false;
		private bool _holdingSpace = false;

		#region IButtonControl implementation

		private DialogResult _dialogResult;
		public DialogResult DialogResult { get { return _dialogResult; } set { _dialogResult = value; } }

		public void NotifyDefault(bool value)
		{
			_isDefault = value;
		}

		public void PerformClick()
		{
			base.OnClick(EventArgs.Empty);
		}

		#endregion IButtonControl implementation

		#region HoverImage

		private Image m_HoverImage;

		[Category("Appearance")]
		[Description("Image to show when the button is hovered over.")]
		public Image HoverImage
		{
			get { return m_HoverImage; }
			set { m_HoverImage = value; if (_hover) Image = value; }
		}

		#endregion HoverImage

		#region DownImage

		private Image m_DownImage;

		[Category("Appearance")]
		[Description("Image to show when the button is depressed.")]
		public Image DownImage
		{
			get { return m_DownImage; }
			set { m_DownImage = value; if (_down) Image = value; }
		}

		#endregion DownImage

		#region NormalImage

		private Image m_NormalImage;

		[Category("Appearance")]
		[Description("Image to show when the button is not in any other state.")]
		public Image NormalImage
		{
			get { return m_NormalImage; }
			set { m_NormalImage = value; if (!(_hover || _down)) Image = value; }
		}

		#endregion NormalImage

		#region overrides

		[Browsable(true)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		[Category("Appearance")]
		[Description("The text associated with the control.")]
		public override string Text { get { return base.Text; } set { base.Text = value; } }

		[Browsable(true)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		[Category("Appearance")]
		[Description("The font used to display text in the control.")]
		public override Font Font { get { return base.Font; } set { base.Font = value; } }

		#endregion overrides

		#region description changes

		[Description("Controls how the ImageButton will handle image placement and control sizing.")]
		public new PictureBoxSizeMode SizeMode { get { return base.SizeMode; } set { base.SizeMode = value; } }

		[Description("Controls what type of border the ImageButton should have.")]
		public new BorderStyle BorderStyle { get { return base.BorderStyle; } set { base.BorderStyle = value; } }

		#endregion description changes

		#region hiding

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Image Image { get { return base.Image; } set { base.Image = value; } }

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new ImageLayout BackgroundImageLayout { get { return base.BackgroundImageLayout; } set { base.BackgroundImageLayout = value; } }

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Image BackgroundImage { get { return base.BackgroundImage; } set { base.BackgroundImage = value; } }

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new String ImageLocation { get { return base.ImageLocation; } set { base.ImageLocation = value; } }

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Image ErrorImage { get { return base.ErrorImage; } set { base.ErrorImage = value; } }

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Image InitialImage { get { return base.InitialImage; } set { base.InitialImage = value; } }

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new bool WaitOnLoad { get { return base.WaitOnLoad; } set { base.WaitOnLoad = value; } }

		#endregion hiding

		#region events

		protected override void OnMouseMove(MouseEventArgs e)
		{
			_hover = true;
			if (_down)
			{
				if ((m_DownImage != null) && (Image != m_DownImage))
					Image = m_DownImage;
			}
			else
				if (m_HoverImage != null)
				Image = m_HoverImage;
			else
				Image = m_NormalImage;
			base.OnMouseMove(e);
		}

		protected override void OnMouseLeave(EventArgs e)
		{
			_hover = false;
			Image = m_NormalImage;
			base.OnMouseLeave(e);
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.Focus();
			OnMouseUp(null);
			_down = true;
			if (m_DownImage != null)
				Image = m_DownImage;
			base.OnMouseDown(e);
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			_down = false;
			if (_hover)
			{
				if (m_HoverImage != null)
					Image = m_HoverImage;
			}
			else
				Image = m_NormalImage;
			base.OnMouseUp(e);
		}

		public override bool PreProcessMessage(ref Message msg)
		{
			if (msg.Msg == WM_KEYUP)
			{
				if (_holdingSpace)
				{
					if ((int)msg.WParam == (int)Keys.Space)
					{
						OnMouseUp(null);
						PerformClick();
					}
					else if ((int)msg.WParam == (int)Keys.Escape
						|| (int)msg.WParam == (int)Keys.Tab)
					{
						_holdingSpace = false;
						OnMouseUp(null);
					}
				}
				return true;
			}
			else if (msg.Msg == WM_KEYDOWN)
			{
				if ((int)msg.WParam == (int)Keys.Space)
				{
					_holdingSpace = true;
					OnMouseDown(null);
				}
				else if ((int)msg.WParam == (int)Keys.Enter)
				{
					PerformClick();
				}
				return true;
			}
			else
				return base.PreProcessMessage(ref msg);
		}

		protected override void OnLostFocus(EventArgs e)
		{
			_holdingSpace = false;
			OnMouseUp(null);
			base.OnLostFocus(e);
		}

		protected override void OnPaint(PaintEventArgs pe)
		{
			base.OnPaint(pe);
			if ((!string.IsNullOrEmpty(Text)) && (pe != null) && (base.Font != null))
			{
				SolidBrush drawBrush = new SolidBrush(base.ForeColor);
				SizeF drawStringSize = pe.Graphics.MeasureString(base.Text, base.Font);
				PointF drawPoint;
				if (base.Image != null)
					drawPoint = new PointF(base.Image.Width / 2 - drawStringSize.Width / 2, base.Image.Height / 2 - drawStringSize.Height / 2);
				else
					drawPoint = new PointF(base.Width / 2 - drawStringSize.Width / 2, base.Height / 2 - drawStringSize.Height / 2);
				pe.Graphics.DrawString(base.Text, base.Font, drawBrush, drawPoint);
			}
		}

		protected override void OnTextChanged(EventArgs e)
		{
			Refresh();
			base.OnTextChanged(e);
		}

		#endregion events
	}
}