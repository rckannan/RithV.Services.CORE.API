using Microsoft.VisualStudio.TestTools.UnitTesting;
using RithV.Services.CORE.API.Queries;
using RithV.Services.CORE.API.Repositories;

namespace RithV.Services.CORE.API.testcases
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]

        public void Debit_WithValidAmount_UpdatesBalance()
        {
            var ctrl = new DataQuery("Data Source=.;Initial Catalog=TestDB;User ID=SA;Password=k@nnan123");
            Assert.IsNotNull(ctrl);
        }

        //[TestMethod]

        //public void Debit_WithValidAmount()
        //{
        //    // Arrange
        //    double beginningBalance = 11.99;
        //    double debitAmount = 4.55;
        //    double expected = 7.44;

        //    // Act
        //    account.Debit(debitAmount);

        //    // Assert
        //    double actual = account.Balance;
        //    Assert.AreEqual(expected, actual, 0.001, "Account not debited correctly");
        //}
    }
}
