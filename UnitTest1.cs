using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.SqlClient;

namespace LagerTests
{
    [TestClass]
    public class LagerTests
    {
        private string connectionString = "Server=ServerName;Database=LagerKapazität;Integrated Security=True;";

        [TestMethod]
        public void Test_SaveDataToDatabase()
        {
            // Arrange
            string ware = "Testware";
            int menge = 10;

            // Act
            SaveDataToDatabase(ware, menge); // Deine Methode aufrufen

            // Assert
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT COUNT(*) FROM dbo.Lager WHERE Ware = @Ware";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Ware", ware);
                    int count = (int)command.ExecuteScalar();
                    Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(1, count, "Die Ware sollte in der Datenbank gespeichert werden.");
                }
            }

            // Clean up: Entferne die Testware
            DeleteTestWare(ware);
        }

        [TestMethod]
        public void Test_UpdateDataInDatabase()
        {
            // Arrange
            string ware = "Testware";
            int initialMenge = 10;
            int neueMenge = 5;

            // Speichern der Testware zuerst
            SaveDataToDatabase(ware, initialMenge);

            // Act
            UpdateDataInDatabase(ware, neueMenge); // Deine Methode aufrufen

            // Assert
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT Menge FROM dbo.Lager WHERE Ware = @Ware";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Ware", ware);
                    int updatedMenge = (int)command.ExecuteScalar();
                    Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(initialMenge + neueMenge, updatedMenge, "Die Menge sollte korrekt aktualisiert werden.");
                }
            }

            // Clean up: Entferne die Testware
            DeleteTestWare(ware);
        }


        [TestMethod]
        public void Test_SaveDataToDatabase_WareExists()
        {
            // Arrange
            string ware = "Testware";
            int menge = 10;

            // Speichere die Testware einmal
            SaveDataToDatabase(ware, menge);

            // Act - Versuche, die gleiche Ware erneut zu speichern
            SaveDataToDatabase(ware, menge);

            // Assert
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT COUNT(*) FROM dbo.Lager WHERE Ware = @Ware";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Ware", ware);
                    int count = (int)command.ExecuteScalar();
                    // Es sollte nur 1 Eintrag für die Ware geben
                    Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(1, count, "Die Ware sollte nicht doppelt in der Datenbank vorhanden sein.");
                }
            }

            // Clean up
            DeleteTestWare(ware);
        }


        [TestMethod]
        public void Test_SaveNewWare()
        {
            // Arrange
            string ware = "NeueWare";
            int menge = 20;

            // Act
            SaveDataToDatabase(ware, menge);

            // Assert
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT Menge FROM dbo.Lager WHERE Ware = @Ware";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Ware", ware);
                    int savedMenge = (int)command.ExecuteScalar();
                    Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(menge, savedMenge, "Die neue Ware sollte mit der korrekten Menge gespeichert werden.");
                }
            }

            // Clean up
            DeleteTestWare(ware);
        }


        [TestMethod]
        public void Test_UpdateExistingWare()
        {
            // Arrange
            string ware = "Testware";
            int initialMenge = 15;
            int neueMenge = 5;

            // Speichern der Testware zuerst
            SaveDataToDatabase(ware, initialMenge);

            // Act - Aktualisiere die Menge der Ware
            UpdateDataInDatabase(ware, neueMenge);

            // Assert
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT Menge FROM dbo.Lager WHERE Ware = @Ware";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Ware", ware);
                    int updatedMenge = (int)command.ExecuteScalar();
                    Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(initialMenge + neueMenge, updatedMenge, "Die Menge sollte korrekt aktualisiert werden.");
                }
            }

            // Clean up
            DeleteTestWare(ware);
        }

        [TestMethod]
        public void Test_DeleteWare()
        {
            // Arrange
            string ware = "TestwareZumLöschen";
            int menge = 10;

            // Speichere die Testware zuerst
            SaveDataToDatabase(ware, menge);

            // Act - Lösche die Ware
            DeleteTestWare(ware);

            // Assert
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT COUNT(*) FROM dbo.Lager WHERE Ware = @Ware";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Ware", ware);
                    int count = (int)command.ExecuteScalar();
                    Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(0, count, "Die Ware sollte korrekt aus der Datenbank entfernt werden.");
                }
            }
        }


        [TestMethod]
        public void Test_WarningForLowStock()
        {
            // Arrange
            string ware = "LowStockWare";
            int menge = 3; // Unter 5, um die Warnung zu testen

            // Act
            SaveDataToDatabase(ware, menge);

            // Assert
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT Menge FROM dbo.Lager WHERE Ware = @Ware";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Ware", ware);
                    int savedMenge = (int)command.ExecuteScalar();
                    Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(menge, savedMenge, "Die Ware sollte mit der korrekten Menge gespeichert werden.");
                }
            }

            // Clean up
            DeleteTestWare(ware);
        }



        private void DeleteTestWare(string ware)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "DELETE FROM dbo.Lager WHERE Ware = @Ware";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Ware", ware);
                    command.ExecuteNonQuery();
                }
            }
        }

        private void SaveDataToDatabase(string ware, int menge)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "INSERT INTO dbo.Lager (Ware, Menge) VALUES (@Ware, @Menge)";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Ware", ware);
                    command.Parameters.AddWithValue("@Menge", menge);
                    command.ExecuteNonQuery();
                }
            }
        }

        private void UpdateDataInDatabase(string ware, int neueMenge)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "UPDATE dbo.Lager SET Menge = Menge + @Menge WHERE Ware = @Ware";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Menge", neueMenge);
                    command.Parameters.AddWithValue("@Ware", ware);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
