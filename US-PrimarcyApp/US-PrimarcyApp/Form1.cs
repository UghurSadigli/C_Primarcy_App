using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using US_PrimarcyApp.Model;

namespace US_PrimarcyApp
{
    public partial class Form1 : Form
    {
        USPrimarcyAppDB db = new USPrimarcyAppDB();
        public Form1()
        {
            InitializeComponent();
        }

        private void medicinecombofill()
        {
            cmbMedicine.Items.AddRange(db.Medicine.Select(m => m.Name).ToArray());

        }
        private void Fillmedicinedatagrid()
        {
            dtgmedicinelist.DataSource =
            
            dtgmedicinelist.DataSource =  db.Medicine.Where(m => m.Name.Contains(cmbMedicine.Text)).Select(md => new
            {
                md.ID,
                medicine_name = md.Name,
                md.Amount,
                firm_name = md.Firms.Name,
                md.Price,
                Receipt = md.isResipt == true ? "reseptli" : "resepsiz",
                md.Prodate,
                md.Expdate
            }).Distinct().ToList(); 


            dtgmedicinelist.Columns[0].Visible = false; 
            dtgmedicinelist.Columns[6].DefaultCellStyle.Format = "dddd,dd MMMM yyyy";
            dtgmedicinelist.Columns[7].DefaultCellStyle.Format = "dddd,dd MMMM yyyy";

            for (int i = 0; i < dtgmedicinelist.RowCount; i++)
            {
                short Amounth = (short)dtgmedicinelist.Rows[i].Cells[2].Value;
                DateTime exdate = (DateTime)dtgmedicinelist.Rows[i].Cells[7].Value;

                if (exdate <= DateTime.Now)
                {
                    dtgmedicinelist.Rows[i].DefaultCellStyle.BackColor = System.Drawing.Color.Orange;
                    dtgmedicinelist.Rows[i].DefaultCellStyle.ForeColor = System.Drawing.Color.White;
                }
                if (Amounth <= 0)
                {
                    dtgmedicinelist.Rows[i].DefaultCellStyle.BackColor = System.Drawing.Color.DarkRed;
                    dtgmedicinelist.Rows[i].DefaultCellStyle.ForeColor = System.Drawing.Color.White;
                }
                if (Amounth <= 0 && exdate<= DateTime.Now)
                {
                    dtgmedicinelist.Rows[i].DefaultCellStyle.BackColor = System.Drawing.Color.Black;
                    dtgmedicinelist.Rows[i].DefaultCellStyle.ForeColor = System.Drawing.Color.White;
                }
            }




        }

        private void tagscombofill()
        {
            cmbTags.Items.AddRange(db.Tags.Select(x => x.Name).ToArray());
        }


        private void addMedicineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddMedicineApp form = new AddMedicineApp();
            form.Show();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            medicinecombofill();
            tagscombofill();
            Fillmedicinedatagrid();
        }

        private void cmbMedicine_SelectedIndexChanged(object sender, EventArgs e)
        {
            Fillmedicinedatagrid();
        }

        private void cmbTags_SelectedIndexChanged(object sender, EventArgs e)
        {
            Fillmedicinedatagrid();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Barcode barcodereader = new Barcode();
            barcodereader.Show();
        }

        private void btnEx_Click(object sender, EventArgs e)
        {
            if (dtgmedicinelist.Rows.Count > 0)
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "PDF (*.pdf)|*.pdf";
                sfd.FileName = "Output.pdf";
                bool fileError = false;
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    if (File.Exists(sfd.FileName))
                    {
                        try
                        {
                            File.Delete(sfd.FileName);
                        }
                        catch (IOException ex)
                        {
                            fileError = true;
                            MessageBox.Show("It wasn't possible to write the data to the disk." + ex.Message);
                        }
                    }
                    if (!fileError)
                    {
                        try
                        {
                            PdfPTable pdfTable = new PdfPTable(dtgmedicinelist.Columns.Count);
                            pdfTable.DefaultCell.Padding = 3;
                            pdfTable.WidthPercentage = 100;
                            pdfTable.HorizontalAlignment = Element.ALIGN_LEFT;

                            foreach (DataGridViewColumn column in dtgmedicinelist.Columns)
                            {
                                PdfPCell cell = new PdfPCell(new Phrase(column.HeaderText));
                                pdfTable.AddCell(cell);
                            }

                            foreach (DataGridViewRow row in dtgmedicinelist.Rows)
                            {
                                foreach (DataGridViewCell cell in row.Cells)
                                {
                                    pdfTable.AddCell(cell.Value.ToString());
                                }
                            }

                            using (FileStream stream = new FileStream(sfd.FileName, FileMode.Create))
                            {
                                Document pdfDoc = new Document(PageSize.A4, 10f, 20f, 20f, 10f);
                                PdfWriter.GetInstance(pdfDoc, stream);
                                pdfDoc.Open();
                                pdfDoc.Add(pdfTable);
                                pdfDoc.Close();
                                stream.Close();
                            }

                            MessageBox.Show("Data Exported Successfully !!!", "Info");
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error :" + ex.Message);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("No Record To Export !!!", "Info");
            }
        }

        private void btnExcel_Click(object sender, EventArgs e)
        {
            // creating Excel Application  
            Microsoft.Office.Interop.Excel._Application app = new Microsoft.Office.Interop.Excel.Application();
            // creating new WorkBook within Excel application  
            Microsoft.Office.Interop.Excel._Workbook workbook = app.Workbooks.Add(Type.Missing);
            // creating new Excelsheet in workbook  
            Microsoft.Office.Interop.Excel._Worksheet worksheet = null;
            // see the excel sheet behind the program  
            app.Visible = true;
            // get the reference of first sheet. By default its name is Sheet1.  
            // store its reference to worksheet  
            worksheet = workbook.Sheets["Sheet1"];
            worksheet = workbook.ActiveSheet;
            // changing the name of active sheet  
            worksheet.Name = "Exported from gridview";
            // storing header part in Excel  
            for (int i = 1; i < dtgmedicinelist.Columns.Count + 1; i++)
            {
                worksheet.Cells[1, i] = dtgmedicinelist.Columns[i - 1].HeaderText;
            }
            // storing Each row and column value to excel sheet  
            for (int i = 0; i < dtgmedicinelist.Rows.Count - 1; i++)
            {
                for (int j = 0; j < dtgmedicinelist.Columns.Count; j++)
                {
                    worksheet.Cells[i + 2, j + 1] = dtgmedicinelist.Rows[i].Cells[j].Value.ToString();
                }
            }
            // save the application  
            workbook.SaveAs("c:\\output.xls", Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlExclusive, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
            // Exit from the application  
            app.Quit();
        }

        private void dtgmedicinelist_RowHeaderMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            int medID = (int)dtgmedicinelist.Rows[e.RowIndex].Cells[0].Value;
            Medicine selectedmedicine = db.Medicine.First( x => x.ID == medID);

            if (selectedmedicine.Amount !=0 && selectedmedicine.Expdate>DateTime.Now)
            {
                pnlSell.Visible = true;
                txtbaymediname.Text = selectedmedicine.Name;
                nmAmounth.Maximum = (short)selectedmedicine.Amount;
            }
        }
        private void addmedicinevisiblefol(string text)
        {
            if (!ckbaymediname.Items.Contains(text))
            {
                ckbaymediname.Items.Add(text);
            }
            else
            {
                MessageBox.Show("drag already add");
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            addmedicinevisiblefol(txtbaymediname.Text + " - " + nmAmounth.Value);
        }

        private void ckbaymediname_SelectedIndexChanged(object sender, EventArgs e)
        {
            int selected = ckbaymediname.SelectedIndex;

            if (selected != -1)
            {
                ckbaymediname.Items.RemoveAt(selected);
            }
        }

        private void btnBuy_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < ckbaymediname.Items.Count; i++)
            {
                string medicineitem = ckbaymediname.Items[i].ToString();
                string medname = medicineitem.Substring(0,medicineitem.LastIndexOf("-"));
                short count = Convert.ToInt16(medicineitem.Substring(medicineitem.LastIndexOf("-") + 1));
                Medicine selectedindex = db.Medicine.First(f => f.Name == medname);
                db.Orders.Add(new Orders
                {
                    MedicineId = selectedindex.ID,
                    PurcasheDate = DateTime.Now,
                    Amounth = count,
                    Price = selectedindex.Price
                });

                selectedindex.Amount -= count;

                db.SaveChanges();


                MessageBox.Show($"medicine name : {selectedindex.Name},Quantity : {selectedindex.Amount}, Price: {selectedindex.Price}");

            }
        }
    }
}
