using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ClothesShop.Models
{
    public class OrderBy
    {
        public string ColumnName { get; set; }
        public string Direction { get; set; }
    }
    public class Paging
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
    public class FilteredColumns
    {
        public string ColumnName { get; set; }
        public string SearchValue { get; set; }
    }
    public class SearchViewModel : Paging
    {
        public OrderBy OrderBy { get; set; }
        public List<FilteredColumns> FilteredColumns { get; set; }
    }
    public class Filtering<T>
    {
        public List<FilteredColumns> Columns { get; set; }
        public string GetValue(string columnName)
        {
            try
            {
                if (Columns.Any(c => c.ColumnName.ToLower() == columnName.ToLower()))
                    return Columns.First(c => c.ColumnName.ToLower() == columnName.ToLower()).SearchValue;
            }
            catch (Exception ex)
            {
                Logging.Services.LogErrorService.Write(Logging.Enums.AppTypes.PresentationLayer, ex);
            }
            return null;
        }
        public IEnumerable<T> Search(string columnName, IEnumerable<T> originalList)
        {
            try
            {
                string value = GetValue(columnName);
                if (string.IsNullOrEmpty(value))
                    return originalList;

                var property = typeof(T).GetProperty(columnName);
                originalList = originalList.Where(x => property.GetValue(x, null) != null && property.GetValue(x, null).ToString().ToLower().Contains(value.ToLower()));
            }
            catch (Exception ex)
            {
                Logging.Services.LogErrorService.Write(Logging.Enums.AppTypes.PresentationLayer, ex);
            }
            return originalList;
        }
        public IEnumerable<T> Search(IEnumerable<T> originalList)
        {
            try
            {
                if (Columns == null || Columns.Count == 0)
                    return originalList;

                foreach (var column in Columns)
                {
                    string columnName = column.ColumnName;
                    if (columnName.IndexOf("_") != -1)
                        continue;

                    string value = GetValue(columnName);
                    if (string.IsNullOrEmpty(value))
                        return originalList;

                    var property = typeof(T).GetProperty(columnName);
                    originalList = originalList.Where(x => property.GetValue(x, null) != null && property.GetValue(x, null).ToString().ToLower().Contains(value.ToLower()));
                }

            }
            catch (Exception ex)
            {
                Logging.Services.LogErrorService.Write(Logging.Enums.AppTypes.PresentationLayer, ex);
            }
            return originalList;
        }
        public IEnumerable<T> OrderBy(OrderBy order, IEnumerable<T> originalList)
        {
            try
            {
                var sortColumn = order != null ? order.ColumnName : "ID";
                var sortColumnDir = order != null ? order.Direction : "DESC";
                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                {
                    var property = typeof(T).GetProperty(sortColumn);

                    ParameterExpression param = Expression.Parameter(typeof(T), string.Empty);
                    Expression<Func<T, object>> sortLambda = Expression.Lambda<Func<T, object>>(Expression.Convert(Expression.Property(param, sortColumn), typeof(object)), new ParameterExpression[] { param });

                    if (sortColumnDir.ToLower() == "desc")
                        originalList = originalList.AsQueryable<T>().OrderByDescending<T, object>(sortLambda);
                    else
                        originalList = originalList.AsQueryable<T>().OrderBy<T, object>(sortLambda);
                }
            }
            catch (Exception ex)
            {
                Logging.Services.LogErrorService.Write(Logging.Enums.AppTypes.PresentationLayer, ex);
            }
            return originalList;
        }
    }

}