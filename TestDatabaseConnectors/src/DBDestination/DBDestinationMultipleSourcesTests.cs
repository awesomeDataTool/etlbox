using ETLBox.Connection;
using ETLBox.DataFlow.Connectors;
using ETLBoxTests.Fixtures;
using ETLBoxTests.Helper;
using System.Collections.Generic;
using Xunit;

namespace ETLBoxTests.DataFlowTests
{
    [Collection("DataFlow")]
    public class DbDestinationMultipleSourcesTests
    {
        public static IEnumerable<object[]> Connections => Config.AllSqlConnections("DataFlow");

        public DbDestinationMultipleSourcesTests(DataFlowDatabaseFixture dbFixture)
        {
        }

        public class MySimpleRow
        {
            public long Col1 { get; set; }
            public string Col2 { get; set; }
        }

        [Theory, MemberData(nameof(Connections))]
        public void TwoMemSourcesIntoDB(IConnectionManager connection)
        {
            //Arrange
            MemorySource<MySimpleRow> source1 = new MemorySource<MySimpleRow>();
            MemorySource<MySimpleRow> source2 = new MemorySource<MySimpleRow>();
            //Act
            source1.DataAsList = new List<MySimpleRow>()
            {
                new MySimpleRow() { Col1 = 1, Col2 = "Test1" },
                new MySimpleRow() { Col1 = 2, Col2 = "Test2" },
            };
            source2.DataAsList = new List<MySimpleRow>()
            {
                new MySimpleRow() { Col1 = 3, Col2 = "Test3" }
            };
            TwoColumnsTableFixture dest2Columns = new TwoColumnsTableFixture(connection, "DBMultipleDestination");

            //Act
            DbDestination<MySimpleRow> dest = new DbDestination<MySimpleRow>(connection, "DBMultipleDestination");

            source1.LinkTo(dest);
            source2.LinkTo(dest);
            source2.Execute();
            source1.Execute();

            dest.Wait();

            //Assert
            dest2Columns.AssertTestData();
        }
    }
}
