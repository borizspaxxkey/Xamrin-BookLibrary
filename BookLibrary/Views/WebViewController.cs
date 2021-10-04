using System;
using Foundation;
using UIKit;
using System.IO;
using SQLite;
using BookLibrary.Views;
using System.Collections.Specialized;
using BookLibrary.Database;
using BookLibrary.Models;
using System.Linq;

namespace BookLibrary.Views
{
    public partial class WebViewController : UIViewController
    {
        static bool UserInterfaceIdiomIsPhone
        {
            get
            {
                return UIDevice.CurrentDevice.UserInterfaceIdiom ==
                       UIUserInterfaceIdiom.Phone;
            }
        }
        protected WebViewController(IntPtr handle) : base(handle)
        {
        }
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            WebView.ShouldStartLoad += HandleShouldStartLoad;

            var sqliteFilename = "BookLibrary.db";
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            string libraryPath = Path.Combine(documentsPath, "..", "Library");
            var databasePath = Path.Combine(libraryPath, sqliteFilename);

            var databaseConn = new SQLiteConnection(databasePath);
            BookDatabase.CreateDatabase(databaseConn);

            var model = BookDatabase.Database.GetItems().ToList();
            var template = new BookLibraryListing() { Model = model };
            var page = template.GenerateString();

            WebView.LoadHtmlString(page, NSBundle.MainBundle.BundleUrl);
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
        }

        bool HandleShouldStartLoad(UIWebView webView, NSUrlRequest request,
                                   UIWebViewNavigationType navigationType)
        {

            const string scheme = "hybrid:";
            if (request.Url.Scheme != scheme.Replace(":", ""))
                return true;

            var resources = request.Url.ResourceSpecifier.Split('?');
            var method = resources[0];
            var parameters = System.Web.HttpUtility.ParseQueryString(resources[1]);

            switch (method)
            {
                case "CreateNewBook":
                    CreateNewBook(webView);
                    break;
                case "EditBookDetails":
                    EditBookDetails(webView, parameters);
                    break;
                case "SaveBookDetails":
                    SaveBookDetails(webView, parameters);
                    break;
                default:
                    break;
            }
            return false;
        }

        void CreateNewBook(UIWebView webView)
        {
            var template = new BookLibraryAddEdit() { Model = new BookItem() };
            var page = template.GenerateString();
            webView.LoadHtmlString(page, NSBundle.MainBundle.BundleUrl);
        }

        void EditBookDetails(UIWebView webView, NameValueCollection parameters)
        {
            var model = BookDatabase.Database.GetItem(Convert.ToInt32(parameters["Id"]));
            var template = new BookLibraryAddEdit() { Model = model };
            var page = template.GenerateString();
            webView.LoadHtmlString(page, NSBundle.MainBundle.BundleUrl);
        }
 
        void SaveBookDetails(UIWebView webView, NameValueCollection parameters)
        {
            var button = parameters["Button"];
            switch (button)
            {
                case "Save":
                    SaveDetailsToDatabase(parameters);
                    break;
                case "Delete":
                    DeleteBookDetails(parameters);
                    break;
                case "Cancel":
                    break;
                default:
                    break;
            }
            var model = BookDatabase.Database.GetItems().ToList();
            var template = new BookLibraryListing() { Model = model };
            webView.LoadHtmlString(template.GenerateString(), NSBundle.MainBundle.BundleUrl);
        }

        void SaveDetailsToDatabase(NameValueCollection parameters)
        {
            var book = new BookItem
            {
                Id = Convert.ToInt32(parameters["Id"]),
                Title = parameters["Title"],
                Author = parameters["Author"],
                Category = parameters["Category"],
                PublishedYear = parameters["PublishedYear"],
                Publisher = parameters["Publisher"],
                NoPages = parameters["NoPages"],
                Isbn = parameters["Isbn"],
                Summary = parameters["Summary"],
                ImageUrl = parameters["ImageUrl"]
            };
            BookDatabase.Database.SaveItem(book);
        }

        void DeleteBookDetails(NameValueCollection parameters)
        {
            BookDatabase.Database.DeleteItem(Convert.ToInt32(parameters["Id"]));
        }
    }
}