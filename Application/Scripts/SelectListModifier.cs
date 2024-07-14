using Microsoft.AspNetCore.Mvc.Rendering;

namespace Application.Scripts
{
    public static class SelectListModifier
    {
        public static List<SelectListItem> InsertPickItem(SelectList selectList, string pickItemName = "---Выбрать---", bool disabled = true)
        {
            List<SelectListItem> list = [.. selectList];
            return InsertPickItem(list, pickItemName, disabled);
        }

        public static List<SelectListItem> InsertPickItem(List<SelectListItem> list, string pickItemName = "---Выбрать---", bool disabled = true)
        {
            list.Insert(0, new(pickItemName, null, true, disabled));
            return list;
        }

        public static List<SelectListItem> InsertEmptyItem(SelectList selectList, string emptyItemName, int position)
        {
            List<SelectListItem> list = [.. selectList];
            return InsertEmptyItem(list, emptyItemName, position);
        }

        public static List<SelectListItem> InsertEmptyItem(List<SelectListItem> list, string emptyItemName, int position)
        {
            list.Insert(position, new(emptyItemName, ""));
            return list;
        }

        public static List<SelectListItem> InsertSelectItems(SelectList selectList, string emptyItemName, string pickItemName = "---Выбрать---")
        {
            List<SelectListItem> list = [.. selectList];
            InsertPickItem(list, pickItemName);
            InsertEmptyItem(list, emptyItemName, 1);

            return list;
        }

        public static List<SelectListItem> CreateBooleanList(string emptyItemName)
        {
            List<SelectListItem> list = [];

            InsertPickItem(list);
            InsertEmptyItem(list, emptyItemName, 1);

            list.Insert(2, new("True", "True"));
            list.Insert(2, new("False", "False"));

            return list;
        }
    }
}
