namespace takeAwayWebApi.Models.Response
{
    public class TagRes
    {

        public TagRes() { }

        public TagRes(int id,string name) {
            tagId = id;
            tagName = name;
        }

        public int tagId { get; set; }

        public string tagName { set; get; }
    }
}