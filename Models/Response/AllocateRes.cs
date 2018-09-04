using DbOpertion.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace takeAwayWebApi.Models.Response
{
    [Serializable]
    public class AllocateRes
    {
        public AllocateRes(List<AllocateView> views) {
            var view = views.First();
            riderId = (int)view.targetRiderId;
            name = view.targetName;
            listFoods = new List<foodName_amount>();
            foreach (var item in views)
            {
                var temp = listFoods.Where(p => p.foodId == item.foodId).ToList();
                if (temp.Count > 0)
                {
                    listFoods.Where(p => p.foodId == item.foodId).First().amount += (int)item.amount;
                }
                else
                {
                    foodName_amount na = new foodName_amount();
                    na.foodId =(int)item.foodId;
                    na.foodName = item.foodName;
                    na.amount = (int)item.amount;
                    listFoods.Add(na);
                }
            }
        }

        public int riderId { get; set; }//targetRiderId
        public string name { get; set;}//targetRiderName
        public List<foodName_amount> listFoods { get; set; }
    }

    [Serializable]
    public class foodName_amount {
        public int foodId { get; set; }

        public string foodName { get; set; }

        public int amount { get; set; }
    }


    public class AllocateMeRes {
        public AllocateMeRes(List<AllocateView> views)
        {
            var view = views.First();
            riderId = (int)view.riderId;
            name = view.riderName;
            allocateId = view.id;
            listFoods = new List<foodName_amount>();
            foreach (var item in views)
            {
                var temp = listFoods.Where(p => p.foodId == item.foodId).ToList();
                if (temp.Count > 0)
                {
                    listFoods.Where(p => p.foodId == item.foodId).First().amount += (int)item.amount;
                }
                else
                {
                    foodName_amount na = new foodName_amount();
                    na.foodId =(int)item.foodId;
                    na.foodName = item.foodName;
                    na.amount = (int)item.amount;
                    listFoods.Add(na);
                }
            }
        }

        public int allocateId { get; set; }
        public int riderId { get; set; }
        public string name { get; set; }
        public List<foodName_amount> listFoods { get; set; }
    }
}