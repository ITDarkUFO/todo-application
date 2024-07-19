using Microsoft.AspNetCore.Mvc.Rendering;

namespace Application.Scripts
{
    public static class SelectListModifier
    {
        public static List<SelectListItem> InsertPickItem(SelectList selectList, string pickItemName = "Выберите вариант", bool selected = true, bool disabled = true)
        {
            List<SelectListItem> list = [.. selectList];
            return InsertPickItem(list, pickItemName, selected, disabled);
        }

        public static List<SelectListItem> InsertPickItem(List<SelectListItem> list, string pickItemName = "Выберите вариант", bool selected = true, bool disabled = true)
        {
            list.Insert(0, new(pickItemName, null, selected, disabled));
            return list;
        }

        public static List<SelectListItem> InsertSelectItem(SelectList selectList, int position, string itemName, string? itemValue = null,
            bool selected = false, bool disabled = false)
        {
            List<SelectListItem> list = [.. selectList];
            return InsertSelectItem(list, position, itemName, itemValue, selected, disabled);
        }

        public static List<SelectListItem> InsertSelectItem(List<SelectListItem> list, int position, string emptyItemName, string? itemValue = null,
            bool selected = false, bool disabled = false)
        {
            if (itemValue is null)
                list.Insert(position, new(emptyItemName, null, selected, disabled));
            else
                list.Insert(position, new(emptyItemName, itemValue, selected, disabled));
            return list;
        }

        public static List<SelectListItem> InsertInitialItems(SelectList selectList, string emptyItemName, string pickItemName = "Выберите вариант", string? emptyItemValue = null)
        {
            List<SelectListItem> list = [.. selectList];
            InsertPickItem(list, pickItemName);
            InsertSelectItem(list, 1, emptyItemName, emptyItemValue);

            return list;
        }

        public static List<SelectListItem> CreateBooleanList(string emptyItemName, string pickItemName = "Выберите вариант", string trueText = "True", string falseText = "False")
        {
            List<SelectListItem> list = [];

            InsertPickItem(list, pickItemName);
            InsertSelectItem(list, 1, emptyItemName);

            list.Insert(2, new(trueText, "True"));
            list.Insert(2, new(falseText, "False"));

            return list;
        }
    }
}
