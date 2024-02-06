using HtmlSerializer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HtmlSerializer
{
    public class HtmlElement
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<string> Attributes { get; set; }
        public List<string> Classes { get; set; }
        public string InnerHtml { get; set; }
        public HtmlElement Parent { get; set; }
        public List<HtmlElement> Children { get; set; }
        public HtmlElement()
        {
            Classes = new List<string>();
            Attributes = new List<string>();
            Children = new List<HtmlElement>();
        }
        public IEnumerable<HtmlElement> Descendants()
        {
            Queue<HtmlElement> queue = new Queue<HtmlElement>();
            queue.Enqueue(this);
            while (queue.Count > 0)
            {
                var currentElement = queue.Dequeue();
                yield return currentElement;

                foreach (var child in currentElement.Children)
                {
                    queue.Enqueue(child);
                }
            }
        }

        public IEnumerable<HtmlElement> Ancestors()
        {
            var currentElement = this.Parent;

            while (currentElement != null)
            {
                yield return currentElement;
                currentElement = currentElement.Parent;
            }
        }
    }
}
public static class HtmlElementExtensions
{
    public static HashSet<HtmlElement> FindElementsBySelector(this HtmlElement element, Selector selector)
    {
        var results = new HashSet<HtmlElement>();
        FindElementsBySelectorRecursive(element, selector, results);
        return results;
    }

    private static void FindElementsBySelectorRecursive(HtmlElement element, Selector selector, HashSet<HtmlElement> results)
    {

        // Check if there is a next selector in the sequence
        if (selector.Child != null)
        {
            // Continue recursively on the filtered descendants with the next selector
            var filteredDescendants = element.Descendants().Where(descendant => descendant.MatchesSelector(selector));
            foreach (var filteredDescendant in filteredDescendants)
            {
                FindElementsBySelectorRecursive(filteredDescendant, selector.Child, results);
            }
        }
        else
        {
            //results.Add(element);
            // If the current selector is the last one, add the filtered descendants to the final result
            results.UnionWith(element.Descendants().Where(descendant => descendant.MatchesSelector(selector)));
        }
    }



    private static bool MatchesSelector(this HtmlElement element, Selector selector)
    {
        return
            (string.IsNullOrEmpty(selector.TagName) || element.Name == selector.TagName) &&
            (string.IsNullOrEmpty(selector.Id) || element.Id == selector.Id) &&
            (selector.Classes.All(cls => element.Classes.Contains(cls)));
    }
}   



