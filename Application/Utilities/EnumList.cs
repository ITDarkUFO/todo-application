using Application.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Reflection;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.ComponentModel.DataAnnotations;

namespace Application.Utilities
{
    public static class Extensions
    {
        /// <summary>
        ///     A generic extension method that aids in reflecting 
        ///     and retrieving any attribute that is applied to an `Enum`.
        /// </summary>
        public static TAttribute? GetAttribute<TAttribute>(this Enum enumValue)
                where TAttribute : Attribute
        {
            return enumValue.GetType()
                            .GetMember(enumValue.ToString())
                            .First()
                            .GetCustomAttribute<TAttribute>();
        }
    }

    public static class EnumList
    {
        public static List<T> GetList<T>() where T : struct, Enum
        {
            return Enum.GetValues(typeof(T)).Cast<T>().ToList();
        }

        //TODO: Функция должна преобразовывать enum или list<enum> в selectlist, где id - название, а имя - name из DisplayAttribute
        //public static SelectList? GetSelectList<T>(IEnumerable<T>? values = null) where T : struct, Enum
        //{
        //    if (!typeof(T).IsEnum)
        //    {
        //        return null;
        //    }

        //    values ??= GetList<T>();


        //    //Dictionary<string, string> enumData = [];

        //    //foreach (var value in values)
        //    //{
        //    //    enumData.Add(typeof(T).GetEnumName(value)!, value.GetType().GetCustomAttribute<DisplayAttribute>()!.Name!);
        //    //}

        //    values.Select(e => new { Id = typeof(T).GetEnumName(e), Name = typeof(T).GetElementType(e). });

        //    return new(enumData, "Key", "Value");
        //}
    }
}
