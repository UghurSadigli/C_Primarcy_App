using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using US_PrimarcyApp.Helpers;
using US_PrimarcyApp.Model;

namespace US_PrimarcyApp
{
    public partial class AddMedicineApp : Form
    {
        USPrimarcyAppDB db = new USPrimarcyAppDB();
        public AddMedicineApp()
        {
            InitializeComponent();
        }
        private void Fillmedicinedatagrid()
        {
            dtgmedicinelist.DataSource = db.Medicine.Select(md => new
            {
                medicine_name = md.Name,
                md.Amount,
                firm_name = md.Firms.Name,
                md.Price,
                Receipt = md.isResipt == true ? "reseptli" : "resepsiz",
                md.Prodate,
                md.Expdate
            }).ToList();

            dtgmedicinelist.Columns[5].DefaultCellStyle.Format = "dddd,dd MMMM yyyy";
            dtgmedicinelist.Columns[6].DefaultCellStyle.Format = "dddd,dd MMMM yyyy";

        }

        public void FillFirmCombo()
        {
            cmbFirms.Items.AddRange(db.Firms.Select( f => f.Name).ToArray());
        }
        public void FillTagsCombo()
        {
            cmbTags.Items.AddRange(db.Tags.Select( f => f.Name).ToArray());
        }
        private void AddMedicineApp_Load(object sender, EventArgs e)
        {
            FillFirmCombo();
            FillTagsCombo();
            Fillmedicinedatagrid();
            cmbFirms.SelectedIndex = 0;
            cmbTags.SelectedIndex = 0;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string Medicinename = txtMedname.Text;
            string firmname = cmbFirms.Text;
            decimal price = nmPrice.Value;
            short amounth = (short) nmAmounth.Value;
            int barcode =Convert.ToInt32(txtBarcode.Text);
            DateTime prodate = dtgProdate.Value;
            DateTime expdate = dtgExpdate.Value;
            string description = rtDescription.Text;
            bool isrec = txtcheck.Checked;

            string[] ar = { Medicinename, firmname,description };


            if (Utilities.IsEmpty(ar))
            {
                int fimrId = FindFirm(firmname);
                Medicine medicine = new Medicine();
                medicine.Name = Medicinename;
                medicine.Price = price;
                medicine.Description = description;
                medicine.Expdate = expdate;
                medicine.Prodate = prodate;
                medicine.Amount = amounth;
                medicine.FirmsId = fimrId;  
                medicine.isResipt = txtcheck.Checked ? true : false;
                medicine.Barcode = barcode; 
                db.Medicine.Add(medicine);  
                db.SaveChanges();
                MessageBox.Show("medicine added succestfully", "Medicine", MessageBoxButtons.OK, MessageBoxIcon.Information);
                db.SaveChanges();
                MedicineAddTag(medicine.ID);
            }
            else
            {
                MessageBox.Show("Please fill all the bank", "Medicine", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public int FindFirm(string firmname)
        {
            Firms selectedfirm = db.Firms.FirstOrDefault(fr => fr.Name == firmname);
            if (selectedfirm == null)
            {
                Firms firm = new Firms();
                firm.Name = firmname;
                db.Firms.Add(firm);
                db.SaveChanges();
                return firm.ID;
            }
            return selectedfirm.ID;

        }
        private void addtagitem()
        {
            string tagname = cmbTags.Text;
            if (!cmbTags.Items.Contains(tagname) && !string.IsNullOrWhiteSpace(tagname))
            {
                ckTagList.Items.Add(tagname,true);
            }
        }

        private void txtBarcode_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar < 48 || e.KeyChar >57) && e.KeyChar !=8)
            {
                e.Handled = true;
            }
            else if (txtBarcode.TextLength >= 13)
            {
                e.Handled = true;
            }
        }

        private void cmbTags_SelectedIndexChanged(object sender, EventArgs e)
        {
            addtagitem();
        }

        private void cmbTags_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                addtagitem();
            }
        }

        private void ckTagList_SelectedIndexChanged(object sender, EventArgs e)
        {
            int selected = ckTagList.SelectedIndex;

            if (selected != -1)
            {
                ckTagList.Items.RemoveAt(selected);   
            }  
        }
        private void MedicineAddTag(int medicineID)
        {
            for (var i = 0; i < ckTagList.Items.Count; i++)
            {
                string tagname = ckTagList.Items[i].ToString();
                int tagID;
                if(chechktagname(tagname))
                {
                    tagID = db.Tags.First(x => x.Name == tagname).ID;
                }
                else
                {
                    Tags tag = db.Tags.Add(new Tags
                    {
                        Name = tagname,
                    });
                    db.SaveChanges();
                    tagID = tag.ID;
                }
                db.Medicine_To_Tags.Add(new Medicine_To_Tags
                {
                    MedicineId = medicineID,
                    TagsId = tagID
                });
                db.SaveChanges();
            } 
        }
        private bool chechktagname(string tg )
        {
            Tags tag = db.Tags.FirstOrDefault(x => x.Name == tg);

            if (tag == null)
            {
                return false;
            }
            return true;
        }
    }
}
