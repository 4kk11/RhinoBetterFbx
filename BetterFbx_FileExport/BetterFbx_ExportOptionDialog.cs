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
		private Button cancelButton = new Button();
		private Button okButton = new Button();

		
		public ExportOptionDialog()
		{
			Resizable = false;
			Title = "FBX Export Options";
			mapZtoY.Text = "Map Rhino Z to FBX Y";
			cancelButton.Text = "Cancel";
			okButton.Text = "OK";

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
		}

		private bool GetCheckboxValue(CheckBox checkBox)
		{
			return checkBox.Checked.HasValue ? checkBox.Checked.Value : false;
		}
	}
}
