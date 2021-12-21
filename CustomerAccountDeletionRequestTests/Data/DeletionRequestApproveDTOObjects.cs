using System;
using System.Collections.Generic;
using System.Text;

namespace CustomerAccountDeletionRequestTests.Data
{
    /// <summary>
    /// Class to serve as a data provider for Unit Tests.
    /// </summary>
    public class DeletionRequestApproveDTOObjects
    {
        public DeletionRequestApproveDTOObjects(){}

        /// <summary>
        /// Used to provide a list of test objects.
        /// Array Args 1: CustomerID - Any Int32 > 0
        /// Array Args 2: StaffID - Any Int32 > 0
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Object[]> GetDeletionRequestApproveDTOObjects()
        {
            return new List<Object[]>
            {
                new object[] { 1, 2 },
                new object[] { 2, 4 },
                new object[] { 3, 8 },
                new object[] { 4, 16 },
                new object[] { 5, 32 }
            };
        }
    }
}
