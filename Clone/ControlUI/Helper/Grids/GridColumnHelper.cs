using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text.RegularExpressions;

namespace MWSManagement.ControlUI.Helper.Grids
{
    public static class GridColumnHelper
    {
        public static List<GridColumnDto> FromModel<T>()
        {
            return typeof(T)
                .GetProperties()
                .Select(property =>
                {
                    var displayAttr = property.GetCustomAttribute<DisplayAttribute>();
                    var gridAttr = property.GetCustomAttribute<GridColumnAttribute>();

                    return new GridColumnDto
                    {
                        Field = property.Name,

                        HeaderText = displayAttr?.Name ?? SplitPascalCase(property.Name),

                        Width = gridAttr?.Width ?? 150,

                        IsPrimaryKey = gridAttr?.IsPrimaryKey ?? false,

                        Visible = gridAttr?.Visible ?? true,

                        TextAlign = gridAttr?.TextAlign,

                        Format = gridAttr?.Format,

                        Type = gridAttr?.Type,

                        AllowFiltering = gridAttr?.AllowFiltering ?? true,

                        AllowSorting = gridAttr?.AllowSorting ?? true,
                        EditType = gridAttr?.EditType,
                        AllowEditing = gridAttr?.AllowEditing ?? true,
                    };
                })
                .ToList();
        }

        private static string SplitPascalCase(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return text;

            return Regex.Replace(text, "([a-z])([A-Z])", "$1 $2");
        }
    }
}