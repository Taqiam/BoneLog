using BoneLog.Models;

namespace BoneLog.Tools;

public static class PostIndexCategoriesHelper
{
    public static Category[] BuildCategory(this IReadOnlyList<PostIndex> posts)
    {
        var roots = new Dictionary<string, Node>(StringComparer.OrdinalIgnoreCase);

        foreach (var post in posts)
        {
            if (string.IsNullOrWhiteSpace(post.Category))
                continue;

            var segments = post.Category.Split(PathExtensions.CategorySeparator, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            if (segments.Length == 0)
                continue;

            var level = roots;
            Node? node = null;

            foreach (var segment in segments)
            {
                if (!level.TryGetValue(segment, out node))
                {
                    node = new Node(segment);
                    level[segment] = node;
                }

                level = node.Children;
            }

            node!.DirectPosts++;
        }

        return roots.Values.OrderBy(n => n.Title, StringComparer.OrdinalIgnoreCase).Select(ToCategory).ToArray();


    }

    static Category ToCategory(Node node)
    {
        var children = node.Children.Values.OrderBy(c => c.Title, StringComparer.OrdinalIgnoreCase).Select(ToCategory).ToArray();

        var childTotal = children.Sum(c => c.NumberOfPosts);

        return new()
        {
            Title = node.Title,
            NumberOfPosts = node.DirectPosts + childTotal,
            SubCategories = children.Length > 0 ? children : null
        };
    }

    record Node(string title)
    {
        public string Title { get; } = title;
        public int DirectPosts { get; set; }
        public Dictionary<string, Node> Children { get; } = new(StringComparer.OrdinalIgnoreCase);
    }
}
