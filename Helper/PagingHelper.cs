using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace takeAwayWebApi.Helper
{
    public class QueryParameters
    {
        public QueryParameters(){}

        public string order { get; set; }
        public int page { get; set; }
        public int rows { get; set; }
        public string sort { get; set; }
    }

    public class DataGrid<T>
    {
        /// <summary>
        /// 默认构造函数
        /// </summary>
        public DataGrid() { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="rows">表格数据</param>
        public DataGrid(List<T> rows)
        {
            this.rows = rows;
        }
        private List<T> _rows = new List<T>();

        /// <summary>
        /// 表格数据
        /// </summary>
        public List<T> rows
        {
            get { return _rows; }
            set
            {
                if (value != null)
                {
                    _rows = value;
                }
            }
        }

        /// <summary>
        /// 总记录数
        /// </summary>
        public int total { get; set; }

        public static DataGrid<T> QueryWithParameters(IEnumerable<T> queryable, QueryParameters queryParameters)
        {
            if (queryable == null)
            {
                return new DataGrid<T>();
            }
            queryParameters = queryParameters ?? new QueryParameters();
            IQueryable<T> source = Queryable.AsQueryable<T>(queryable);

            var page = queryParameters.page;
            var rows = queryParameters.rows;
            var sort = queryParameters.sort;
            var order = queryParameters.order;

            int num = source.Count();

            int count = (page - 1) * rows;

            IQueryable<T> s = source;

            if (!string.IsNullOrWhiteSpace(sort))
            {
                var sortExpression = Expression.Parameter(source.ElementType);
                var selector = Expression.Lambda(Expression.PropertyOrField(sortExpression, sort), sortExpression);
                if (order.ToLower() == "asc")
                {
                    s = (IQueryable<T>)source.Provider.CreateQuery(Expression.Call(typeof(Queryable), "OrderBy", new Type[] { source.ElementType, selector.Body.Type }, source.Expression, selector));
                }
                else if (order.ToLower() == "desc")
                {
                    s = (IQueryable<T>)source.Provider.CreateQuery(Expression.Call(typeof(Queryable), "OrderByDescending", new Type[] { source.ElementType, selector.Body.Type }, source.Expression, selector));
                }
            }

            //方法1
            s = count == 0 ? s.Take(rows) : s.Skip(count).Take(rows);
            //方法2
            s = count == 0 ? Queryable.Take<T>(s, rows) : Queryable.Take<T>(Queryable.Skip<T>(s, count), rows);

            return new DataGrid<T>()
            {
                total = num,
                rows = s.ToList()
            };
        }

        public static DataGrid<T> QueryWithParametersMine(IEnumerable<T> queryable,string orderMine, int index, int size,string sortMine)
        {
            QueryParameters queryParameters = new QueryParameters();
            queryParameters.order = orderMine;
            queryParameters.page = index;
            queryParameters.rows = size;
            queryParameters.sort = sortMine;


            if (queryable == null)
            {
                return new DataGrid<T>();
            }
            queryParameters = queryParameters ?? new QueryParameters();
            IQueryable<T> source = Queryable.AsQueryable<T>(queryable);

            var page = queryParameters.page;
            var rows = queryParameters.rows;
            var sort = queryParameters.sort;
            var order = queryParameters.order;

            int num = source.Count();

            int count = (page - 1) * rows;

            IQueryable<T> s = source;

            if (!string.IsNullOrWhiteSpace(sort))
            {
                var sortExpression = Expression.Parameter(source.ElementType);
                var selector = Expression.Lambda(Expression.PropertyOrField(sortExpression, sort), sortExpression);
                if (order.ToLower() == "asc")
                {
                    s = (IQueryable<T>)source.Provider.CreateQuery(Expression.Call(typeof(Queryable), "OrderBy", new Type[] { source.ElementType, selector.Body.Type }, source.Expression, selector));
                }
                else if (order.ToLower() == "desc")
                {
                    s = (IQueryable<T>)source.Provider.CreateQuery(Expression.Call(typeof(Queryable), "OrderByDescending", new Type[] { source.ElementType, selector.Body.Type }, source.Expression, selector));
                }
            }

            //方法1
            s = count == 0 ? s.Take(rows) : s.Skip(count).Take(rows);
            //方法2
            s = count == 0 ? Queryable.Take<T>(s, rows) : Queryable.Take<T>(Queryable.Skip<T>(s, count), rows);

            return new DataGrid<T>()
            {
                total = num,
                rows = s.ToList()
            };
        }
    }
}