namespace BoneLog.Tools;

public static class PagingHelper
{
    public const int DEFAULT_ITEMS_PER_PAGE = 10;

    public static int GetPageCount(int itemCount, int itemsPerPage = DEFAULT_ITEMS_PER_PAGE) => itemCount == 0 ? 0 : (int)Math.Ceiling((double)itemCount / itemsPerPage);
    public static int ClampPage(int page, int pageCount) => Math.Clamp(page, 1, Math.Max(pageCount, 1));
    public static T[] GetPage<T>(IReadOnlyList<T> items, int currentPage, int itemsPerPage = DEFAULT_ITEMS_PER_PAGE) => items.Skip((currentPage - 1) * itemsPerPage).Take(itemsPerPage).ToArray();
}
