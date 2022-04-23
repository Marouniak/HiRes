/**
 * FILE: SortableGrid.cs
 * 
 * PROJECT: IssueTracker
 * 
 * ABSTRACT: Sortable grid web UI control
 * 
 * LEGAL: (c)2001 Eurosoft International Inc.
 *
 * Revision history:
 */

using System;
using System.Drawing;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace HiRes.Web.Common.Controls {
	
	public class SortableGrid : DataGrid {
		// Constructor that sets some styles and graphical properties	
		public SortableGrid() {
			// Default settings for the grid Pager
			PagerStyle.Mode = PagerMode.NumericPages;
			
			PagerStyle.PageButtonCount = 10;
			//PagerStyle.HorizontalAlign = HorizontalAlign.Right;

			// Default settings for pagination
			AllowPaging = true;
			PageSize = 7;
		
			// Other visual default settings
			GridLines = GridLines.None;
			//CellSpacing = 2;
			//CellPadding = 2;
			//BorderColor = Color.Black;
			//BorderStyle = BorderStyle.Solid;
			//BorderWidth = (Unit) 1;
			//ForeColor = Color.Black;
			//Font.Size = FontUnit.XSmall;
			//Font.Name = "Verdana";

			// Settings for normal items (all or odd-only rows)
			//ItemStyle.BackColor = Color.Beige;

			// Settings for alternating items (none or even-only rows)
			//AlternatingItemStyle.BackColor = Color.PaleGoldenrod;

			// Settings for heading  
			//HeaderStyle.Font.Bold = true;
			//HeaderStyle.BackColor = Color.Brown;
			//HeaderStyle.ForeColor = Color.White;
			//HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
			 // HeaderStyle.CssClass = "nPricesLink";
			// Sorting
			AllowSorting = true;
			Attributes["SortedAscending"] = "yes";

			// Set event handlers
			ItemCreated += new DataGridItemEventHandler(OnItemCreated);
			SortCommand += new DataGridSortCommandEventHandler(OnSortCommand);
			PageIndexChanged += new DataGridPageChangedEventHandler(OnPageIndexChanged);
		}

		// PROPERTY: SortExpression
		public String SortExpression {
			get { return Attributes["SortExpression"]; }
			set { Attributes["SortExpression"] = value; }
		}


		// PROPERTY: IsSortedAscending
		public bool IsSortedAscending {
			get { return Attributes["SortedAscending"]=="yes"; }
			set { Attributes["SortedAscending"] = (value ?"yes" :"no"); }
		}


		// PROPERTY: PagerCurrentPageCssClass
		String m_PagerCurrentPageCssClass = "";
		public String PagerCurrentPageCssClass {
			get { return m_PagerCurrentPageCssClass; }
			set { m_PagerCurrentPageCssClass = value; }
		}

		// PROPERTY: PagerOtherPageCssClass
		String m_PagerOtherPageCssClass = "";
		public String PagerOtherPageCssClass {
			get { return m_PagerOtherPageCssClass; }
			set { m_PagerOtherPageCssClass = value; }
		}


		// EVENT HANDLER: ItemCreated			
		public void OnItemCreated(Object sender, DataGridItemEventArgs e) {
			// Get the newly created item
			ListItemType itemType = e.Item.ItemType;
			
	
			// PAGER
			if (itemType == ListItemType.Pager) {
				TableCell pager = (TableCell) e.Item.Controls[0];
		
				// Enumerates all the items in the pager...
				for (int i=0; i<pager.Controls.Count; i+=2) {
					// It can be either a Label or a Link button
					try {
						LinkButton h = (LinkButton) pager.Controls[i];
						h.Text = "[ " + h.Text + " ]";
						h.CssClass = m_PagerOtherPageCssClass;					} 
					catch {
						Label l = (Label) pager.Controls[i];
						l.Text = "<b>" + l.Text+"</b>"; // change at 05.06.2002
						l.ForeColor = Color.Red; // change at 05.06.2002
					

						if (m_PagerCurrentPageCssClass == "") {
							l.ForeColor = Color.Black; //change at 05.06.2002 Blue
							l.Font.Bold = true;
						}
						else
							l.CssClass = m_PagerCurrentPageCssClass;
					}
				}
			}

			// HEADER
			if (itemType == ListItemType.Header) {
				String strSortExpression = Attributes["SortExpression"];
				bool m_SortedAscending = (Attributes["SortedAscending"]=="yes");
				String strOrder = (m_SortedAscending ? " 5" : " 6");

				for (int i = 0; i < Columns.Count; i++) {
					// draw to reflect sorting
					try{
						if (strSortExpression.ToLower() == Columns[i].SortExpression.ToLower()) {
							TableCell cell = e.Item.Cells[i];
							Label lblText = new Label();
							lblText.Font.Name = cell.Font.Name;
							lblText.Font.Size = cell.Font.Size;
							lblText.ForeColor = cell.ForeColor; 
							//lblText.CssClass = "nPricesLink";
							lblText.Text		= cell.Text;
							cell.Controls.Add(lblText);
							Label lblSorted = new Label();
							lblSorted.Font.Name = "webdings";
							lblSorted.Font.Size = FontUnit.XSmall;
							lblSorted.ForeColor = Color.GhostWhite;// Red; 
							//lblSorted.CssClass = "nPricesLink";
							lblSorted.Text = strOrder;
							cell.Controls.Add(lblSorted);
						}
					} catch (Exception) {
					}
				}	
			}
		}


		// EVENT HANDLER: SortCommand
		public void OnSortCommand(Object sender, DataGridSortCommandEventArgs e) {
			String strSortExpression = Attributes["SortExpression"];
			String strSortedAscending = Attributes["SortedAscending"];

			// Sets the new sorting field
			Attributes["SortExpression"] = e.SortExpression;

			// Sets the order (defaults to ascending). If you click on the
			// sorted column, the order reverts.
			Attributes["SortedAscending"] = "yes";
			if (e.SortExpression == strSortExpression)
				Attributes["SortedAscending"] = (strSortedAscending=="yes" ?"no" :"yes"); 
		}


		// EVENT HANDLER: PageIndexChanged
		public void OnPageIndexChanged(Object sender, DataGridPageChangedEventArgs e) {
			CurrentPageIndex = e.NewPageIndex;
		}

		protected override void Render(HtmlTextWriter output) {
			base.Render(output);
		}
	}
}
