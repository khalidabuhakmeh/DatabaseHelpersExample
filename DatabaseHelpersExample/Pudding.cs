using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NPoco;
using Xunit;

namespace DatabaseHelpersExample
{
    public class Pudding
    {
        [Fact]
        public void Can_create_and_manipulate_sql_database()
        {
            using (var config = new LocalDb("test_database"))
            {
                var database = new Database(config.OpenConnection());

                database.Execute(
                @"CREATE TABLE Person
                (
                    Id int IDENTITY PRIMARY KEY,
                    Name varchar(60) NULL,
                );");

                database.Insert(new Person { Id = 1, Name = "Khalid Abuhakmeh" });
                var result = database.Query<Person>("select * from Person");
                Assert.True(result.Any());
            }
        }
    }

    public class Person
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
