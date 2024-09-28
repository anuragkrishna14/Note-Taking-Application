using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.OleDb;

namespace Note_Taking_Application
{
    public partial class Form1 : Form
    {
        DataTable notes = new DataTable();
        OleDbConnection dbCon = new OleDbConnection(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=..\..\Database\Notes.accdb");
        bool editFlag;
        string prevTitle;
        int indexOfPrevTitle;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            notes.Columns.Add("Title");
            notes.Columns.Add("Note");

            dbCon.Open();
            OleDbCommand cmdRead = new OleDbCommand("Select Title from Notes", dbCon);
            OleDbDataReader reader = cmdRead.ExecuteReader();
            while (reader.Read())
            {
                lbNotes.Items.Add(reader["Title"].ToString());
            }
            dbCon.Close();
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            txtTitle.Text = "";
            txtNote.Text = "";
            txtTitle.Focus();
            afterEdit();
            lbNotes.SelectedItem = null;
        }

        void saveNew()
        {
            if (txtTitle.Text != "")
            {
                bool saveFlag = true;
                for (int i = 0; i < lbNotes.Items.Count; i++)
                {
                    if (string.Equals(lbNotes.Items[i].ToString(), txtTitle.Text, StringComparison.CurrentCultureIgnoreCase))
                    {
                        saveFlag = false;
                    }
                }

                if (saveFlag)
                {
                    dbCon.Open();
                    OleDbCommand cmdInsert = new OleDbCommand("Insert Into Notes ([Title], [Note]) values('" + txtTitle.Text + "','" + txtNote.Text + "')", dbCon);
                    cmdInsert.ExecuteNonQuery();
                    lbNotes.Items.Add(txtTitle.Text.ToString());
                    dbCon.Close();
                    txtTitle.Text = "";
                    txtNote.Text = "";
                    txtTitle.Focus();
                }
                else
                {
                    MessageBox.Show("This Title is already in the list. Please give any different Title", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                MessageBox.Show("Please give Title to the Note", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        void saveEdited()
        {
            try
            {
                dbCon.Open();
                string query = "Update Notes set [Title] = '" + txtTitle.Text + "', [Note] = '" + txtNote.Text + "' where [Title] = '" + prevTitle + "'";
                OleDbCommand cmdUpdate = new OleDbCommand(query, dbCon);
                cmdUpdate.ExecuteNonQuery();
                lbNotes.Items[indexOfPrevTitle] = txtTitle.Text;
                txtTitle.Text = "";
                txtNote.Text = "";
                txtTitle.Focus();
                dbCon.Close();
            }
            catch (Exception) { dbCon.Close(); }
        }

        void afterEdit()
        {
            prevTitle = "";
            indexOfPrevTitle = -1;
            editFlag = false;
            btnEdit.Visible = false;
            txtTitle.ReadOnly = false;
            txtNote.ReadOnly = false;
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            txtTitle.ReadOnly = false;
            txtNote.ReadOnly = false;
            prevTitle = txtTitle.Text;
            indexOfPrevTitle = lbNotes.Items.IndexOf(prevTitle);
            txtNote.Focus();
            txtNote.Select(0, 0);
            //txtNote.SelectionStart = txtNote.Text.Length;
            //lbNotes.SelectedItem = null;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (editFlag)
            {
                saveEdited();
                afterEdit();
            }
            else
            {
                saveNew();
                afterEdit();
            }
            lbNotes.SelectedItem = null;
        }

        private void btnRead_Click(object sender, EventArgs e)
        {
            dbCon.Open();
            OleDbCommand cmdRead = new OleDbCommand("Select * from Notes where Title='"+lbNotes.Text+"'", dbCon);
            OleDbDataReader reader = cmdRead.ExecuteReader();
            if (reader.Read())
            {
                txtTitle.Text = reader["Title"].ToString();
                txtNote.Text = reader["Note"].ToString();
            }
            dbCon.Close();
            editFlag = true;
            btnEdit.Visible = true;
            btnEdit.Focus();
            txtTitle.ReadOnly = true;
            txtNote.ReadOnly = true;
            //lbNotes.SelectedItem = null;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                String listItem = lbNotes.Text;
                dbCon.Open();
                OleDbCommand cmdInsert = new OleDbCommand("Delete from Notes where Title= '" + listItem + "' ", dbCon);
                cmdInsert.ExecuteNonQuery();
                lbNotes.Items.Remove(listItem);
                dbCon.Close();
                txtTitle.Text = "";
                txtNote.Text = "";
                txtTitle.Focus();
                afterEdit();
            }
            catch (Exception) { dbCon.Close(); }
        }
    }
}
