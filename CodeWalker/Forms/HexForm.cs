using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CodeWalker.Forms
{
	public partial class HexForm : Form
	{
		private byte[] _data;
		private string _fileName;

		public byte[] Data
		{
			get => _data;
			set
			{
				_data = value;
				UpdateTextBoxFromData();
			}
		}

		public string FileName
		{
			get => _fileName;
			set
			{
				_fileName = value;
				UpdateFormTitle();
			}
		}

		public string FilePath { get; set; }
		public bool IgnorePerformanceChecks { get; set; }

		public HexForm()
		{
			InitializeComponent();

			LineSizeDropDown.Text = @"16";
			IgnorePerformanceChecks = false;
		}

		public void LoadData(string filename, string filepath, byte[] data)
		{
			FileName = filename;
			FilePath = filepath;
			Data = data;
		}

		private void UpdateFormTitle()
		{
			Text = _fileName + @" - Hex Viewer - CodeWalker by dexyfex";
		}

		private void UpdateTextBoxFromData()
		{
			if (_data == null)
			{
				HexTextBox.Text = "";
				return;
			}

			if (_data.Length > 5 * 1048576 && !IgnorePerformanceChecks)
			{
				HexTextBox.Text =
					@"[File size > 5MB - Not shown due to performance limitations - Please use an external viewer for this file.]";
				return;
			}

			Cursor = Cursors.WaitCursor;
			bool isHex = LineSizeDropDown.Text != @"Text";

			if (!isHex)
			{
				string text = Encoding.UTF8.GetString(_data);
				HexTextBox.Text = text;
			}
			else
			{
				int charactersPerLine = int.Parse(LineSizeDropDown.Text);
				int lines = _data.Length / charactersPerLine + (_data.Length % charactersPerLine > 0 ? 1 : 0);
				StringBuilder hexB = new StringBuilder();
				StringBuilder texB = new StringBuilder();
				StringBuilder finB = new StringBuilder();

				for (int i = 0; i < lines; i++)
				{
					int pos = i * charactersPerLine;
					int poslim = pos + charactersPerLine;
					hexB.Clear();
					texB.Clear();
					hexB.AppendFormat("{0:X4}: ", pos);
					for (int c = pos; c < poslim; c++)
					{
						if (c < _data.Length)
						{
							byte b = _data[c];
							hexB.AppendFormat("{0:X2} ", b);
							if (char.IsControl((char)b))
							{
								texB.Append(".");
							}
							else
							{
								texB.Append(Encoding.ASCII.GetString(_data, c, 1));
							}
						}
						else
						{
							hexB.Append("   ");
							texB.Append(" ");
						}
					}

					finB.AppendLine(hexB + "| " + texB);
				}

				HexTextBox.Text = finB.ToString();
			}

			Cursor = Cursors.Default;
		}

		private void LineSizeDropDown_SelectedIndexChanged(object sender, EventArgs e)
		{
			UpdateTextBoxFromData();
		}

		private void ignoreChecks_CheckedChanged(object sender, EventArgs e)
		{
			IgnorePerformanceChecks = ignoreChecks.Checked;
			UpdateTextBoxFromData();
		}
	}
}
