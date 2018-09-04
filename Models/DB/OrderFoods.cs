using System;

namespace DbOpertion.Models
{
    [Serializable]
    public class OrderFoods
    {
        /// <summary>
        ///
        /// </summary>
        public Int32 id { get; set; }
        /// <summary>
        ///
        /// </summary>
        public Int32? orderId { get; set; }
        /// <summary>
        ///
        /// </summary>
        public Int32? foodId { get; set; }
        /// <summary>
        ///
        /// </summary>
        public Int32? amount { get; set; }
        /// <summary>
        ///
        /// </summary>
        public Boolean? isDeleted { get; set; }
        /// <summary>
        ///
        /// </summary>
        public String LastName { get; set; }
        /// <summary>
        ///
        /// </summary>
        public Decimal? LastPrice { get; set; }

}
}
