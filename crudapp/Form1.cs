using System;
using System.Data;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace crudapp
{
    public partial class Form1 : Form
    {
        private DataTable dataTable; // DataTable to hold the data from car_details

        public Form1()
        {
            InitializeComponent();
            this.Load += new EventHandler(Form1_Load);
            BtnEdit.Enabled = false; // Disable Edit button by default
            DataGridView1.SelectionChanged += DataGridView1_SelectionChanged; // Attach selection changed event
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            DisplayData();
        }

        private void DisplayData()
        {
            string connectionString = "server=localhost; user id=root; password=; database=crudapp;";
            string query = "SELECT * FROM car_details";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                using (MySqlDataAdapter adapter = new MySqlDataAdapter(query, connection))
                {
                    dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    DataGridView1.DataSource = dataTable;
                }
            }
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            // Validate input fields
            if (string.IsNullOrWhiteSpace(carBrandInput.Text) ||
                string.IsNullOrWhiteSpace(carColor.Text) ||
                string.IsNullOrWhiteSpace(carPayment.Text))
            {
                MessageBox.Show("Please fill in all fields.");
                return;
            }

            string connectionString = "server=localhost; user id=root; password=; database=crudapp;";
            string insertQuery = "INSERT INTO car_details (carBrand, carColor, payment) VALUES (@value1, @value2, @value3)";
            string selectQuery = "SELECT * FROM car_details WHERE id = LAST_INSERT_ID()";

            string value1 = carBrandInput.Text;
            string value2 = carColor.Text;
            string value3 = carPayment.Text;

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                using (MySqlCommand insertCommand = new MySqlCommand(insertQuery, connection))
                {
                    insertCommand.Parameters.AddWithValue("@value1", value1);
                    insertCommand.Parameters.AddWithValue("@value2", value2);
                    insertCommand.Parameters.AddWithValue("@value3", value3);

                    connection.Open();
                    int result = insertCommand.ExecuteNonQuery();

                    if (result > 0)
                    {
                        MessageBox.Show("Record added successfully.");

                        // Fetch the newly inserted row
                        MySqlCommand selectCommand = new MySqlCommand(selectQuery, connection);
                        using (MySqlDataReader reader = selectCommand.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Create a new DataRow with the retrieved data
                                DataRow newRow = dataTable.NewRow();
                                newRow["id"] = reader["id"];
                                newRow["carBrand"] = reader["carBrand"];
                                newRow["carColor"] = reader["carColor"];
                                newRow["payment"] = reader["payment"];
                                dataTable.Rows.Add(newRow);
                            }
                        }

                        // Clear input fields after adding
                        carBrandInput.Text = "";
                        carColor.Text = "";
                        carPayment.Text = "";

                        // Refresh the DataGridView display
                        DataGridView1.Refresh();
                    }
                    else
                    {
                        MessageBox.Show("Error adding record.");
                    }
                }
            }
        }


        private void BtnEdit_Click(object sender, EventArgs e)
        {
            // Check if a row is selected (should be redundant due to button being disabled if no row is selected)
            if (DataGridView1.SelectedRows.Count > 0)
            {
                // Get the selected row
                DataGridViewRow selectedRow = DataGridView1.SelectedRows[0];

                // Populate input fields with data from selected row
                carBrandInput.Text = selectedRow.Cells["carBrand"].Value.ToString();
                carColor.Text = selectedRow.Cells["carColor"].Value.ToString();
                carPayment.Text = selectedRow.Cells["payment"].Value.ToString();

                // Enable BtnSave
                BtnSave.Visible = true;

                BtnAdd.Visible = false;
            }
            else
            {
                MessageBox.Show("Please select a row to edit.");
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (DataGridView1.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = DataGridView1.SelectedRows[0];
                int id = Convert.ToInt32(selectedRow.Cells["id"].Value);

                string value1 = carBrandInput.Text;
                string value2 = carColor.Text;
                string value3 = carPayment.Text;

                string connectionString = "server=localhost; user id=root; password=; database=crudapp;";
                string updateQuery = "UPDATE car_details SET carBrand = @carBrand, carColor = @carColor, payment = @payment WHERE id = @id";

                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    using (MySqlCommand command = new MySqlCommand(updateQuery, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        command.Parameters.AddWithValue("@carBrand", value1);
                        command.Parameters.AddWithValue("@carColor", value2);
                        command.Parameters.AddWithValue("@payment", value3);

                        connection.Open();
                        int result = command.ExecuteNonQuery();

                        if (result > 0)
                        {
                            MessageBox.Show("Record updated successfully.");
                            carBrandInput.Text = "";
                            carColor.Text = "";
                            carPayment.Text = "";

                            BtnSave.Visible = false;

                            DisplayData(); // Refresh DataGridView

                            BtnAdd.Visible = true;
                        }
                    }
                }
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            // Check if a row is selected
            if (DataGridView1.SelectedRows.Count > 0)
            {
                // Get the selected row
                DataGridViewRow selectedRow = DataGridView1.SelectedRows[0];

                // Get the ID of the selected row
                int id = Convert.ToInt32(selectedRow.Cells["id"].Value);

                // Confirmation dialog
                DialogResult result = MessageBox.Show("Are you sure you want to delete this record?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    // Delete query
                    string connectionString = "server=localhost; user id=root; password=; database=crudapp;";
                    string deleteQuery = "DELETE FROM car_details WHERE id = @id";

                    using (MySqlConnection connection = new MySqlConnection(connectionString))
                    {
                        using (MySqlCommand command = new MySqlCommand(deleteQuery, connection))
                        {
                            command.Parameters.AddWithValue("@id", id);

                            connection.Open();
                            int rowsAffected = command.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                MessageBox.Show("Record deleted successfully.");

                                // Refresh DataGridView
                                DisplayData();
                            }
                            else
                            {
                                MessageBox.Show("Error deleting record.");
                            }
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a row to delete.");
            }
        }


        private void DataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            // Enable BtnEdit if a row is selected
            BtnEdit.Enabled = DataGridView1.SelectedRows.Count > 0;
        }

        private void DataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Handle the cell click event if needed
        }

        private void Form1_Load_1(object sender, EventArgs e)
        {
        }

    }
}
