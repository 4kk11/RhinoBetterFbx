using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eto.Forms;

namespace BetterFbx_FileExport
{
	class ExportOptionDialog : Dialog<DialogResult>
	{
		private const int DefaultPadding = 5;
		private static readonly Eto.Drawing.Size DefaultSpacing = new Eto.Drawing.Size(2, 2);

		private CheckBox mapZtoY = new CheckBox();
		private CheckBox isAscii = new CheckBox();
		private Button cancelButton = new Button();
		private Button okButton = new Button();

		
		public ExportOptionDialog()
		{
			Resizable = false;
			Title = "FBX Export Options";
			mapZtoY.Text = "Map Rhino Z to FBX Y";
			isAscii.Text = "Output in Ascii format.";
			cancelButton.Text = "Cancel";
			okButton.Text = "OK";

			okButton.Click += OkButton_Click;
			cancelButton.Click += CancelButton_Click;

			TabControl tabControl = new TabControl();

			TabPage formattingPage = new TabPage()
			{
				Text = "Formatting",
				Content = new TableLayout()
				{
					Padding = DefaultPadding,
					Spacing = DefaultSpacing,
					Rows =
					{
						new TableRow(mapZtoY),
						new TableRow(isAscii),
					},
				},
			};

			tabControl.Pages.Add(formattingPage);

			this.Content = new TableLayout()
			{
				Padding = DefaultPadding,
				Spacing = DefaultSpacing,
				Rows =
				{
					new TableRow(tabControl),
					new TableRow(new TableLayout()
					{
						Padding = DefaultPadding,
						Spacing = DefaultSpacing,
						Rows =
						{
							new TableRow(new TableCell(cancelButton, true), new TableCell(okButton, true)),
						},
					})
				},
			};

		}

		private void DialogToOptions()
		{
			BetterFbx_FileExportPlugin.MapRhinoZToFbxY = GetCheckboxValue(mapZtoY);
			BetterFbx_FileExportPlugin.isAsciiFormat = GetCheckboxValue(isAscii);
		}

		private bool GetCheckboxValue(CheckBox checkBox)
		{
			return checkBox.Checked.HasValue ? checkBox.Checked.Value : false;
		}

		private void CancelButton_Click(object sender, EventArgs e)
		{
			this.Close(DialogResult.Cancel);
		}

		private void OkButton_Click(object sender, EventArgs e)
		{
			DialogToOptions();
			this.Close(DialogResult.Ok);
		}
	}
}
