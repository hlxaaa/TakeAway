using System;

namespace DbOpertion.Models
{
    [Serializable]
    public class FoodView
    {
        /// <summary>
        ///
        /// </summary>
        public DateTime? FoodTime { get; set; }
        /// <summary>
        ///
        /// </summary>
        public Boolean? isOn { get; set; }
        /// <summary>
        ///
        /// </summary>
        public Boolean? isDeleted { get; set; }
        /// <summary>
        ///
        /// </summary>
        public Int32 id { get; set; }
        /// <summary>
        ///
        /// </summary>
        public String foodName { get; set; }
        /// <summary>
        ///
        /// </summary>
        public Decimal? foodPrice { get; set; }
        /// <summary>
        ///
        /// </summary>
        public Int32? foodTagId { get; set; }
        /// <summary>
        ///
        /// </summary>
        public String foodTagName { get; set; }
        /// <summary>
        ///
        /// </summary>
        public String foodText { get; set; }
        /// <summary>
        ///
        /// </summary>
        public Boolean? isMain { get; set; }
        /// <summary>
        ///
        /// </summary>
        public String foodImg { get; set; }
        /// <summary>
        ///
        /// </summary>
        public Boolean? isThisWeek { get; set; }
        /// <summary>
        ///
        /// </summary>
        public Decimal? deposit { get; set; }
        /// <summary>
        ///
        /// </summary>
        public Boolean? isWeek { get; set; }
        /// <summary>
        ///
        /// </summary>
        public Int32? secondTag { get; set; }
        /// <summary>
        ///
        /// </summary>
        public Decimal? star { get; set; }
        /// <summary>
        ///
        /// </summary>
        public Int32? starcount { get; set; }
        /// <summary>
        ///
        /// </summary>
        public String secondTagName { get; set; }

}
}
