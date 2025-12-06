using System.Web;
using System.Web.Optimization;

namespace VentasWeb
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            // ============================
            // JQUERY
            // ============================
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-3.3.1.min.js"));

            // VALIDACIONES
            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // MODERNIZR
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            // ============================
            // BOOTSTRAP 4.6
            // ============================
            bundles.Add(new StyleBundle("~/Content/bootstrap").Include(
                      "~/Content/bootstrap.min.css"
            ));
            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.bundle.min.js"
            ));

            // ============================
            // CSS PRINCIPAL DEL SITIO
            // ============================
            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/Site.css"
            ));

            // ============================
            // CSS PLUGINS
            // ============================
            bundles.Add(new StyleBundle("~/Content/PluginsCSS").Include(
                      "~/Content/Plugins/datatables/css/jquery.dataTables.min.css",
                      "~/Content/Plugins/datatables/css/responsive.dataTables.min.css",
                      "~/Content/Plugins/fontawesome-free-5.15.2/css/all.min.css",
                      "~/Content/Plugins/sweetalert2/css/sweetalert.css",
                      "~/Content/Plugins/jquery-ui-1.12.1/jquery-ui.min.css",
                      "~/Content/Plugins/jquery-ui-1.12.1/jquery-ui-timepicker-addon.css",
                      "~/Content/Plugins/Bootstrap-Duallistbox/css/bootstrap-duallistbox.min.css",

                      // ✅ Select2 CSS
                      "~/Content/Plugins/select2.min.css",
                      "~/Content/Plugins/select2-bootstrap4.min.css"
            ));

            // ============================
            // JS PLUGINS
            // ============================
            bundles.Add(new ScriptBundle("~/Content/PluginsJS").Include(
                     "~/Content/Plugins/datatables/js/jquery.dataTables.min.js",
                     "~/Content/Plugins/datatables/js/dataTables.responsive.min.js",
                     "~/Content/Plugins/fontawesome-free-5.15.2/js/all.min.js",
                     "~/Content/Plugins/sweetalert2/js/sweetalert.js",
                     "~/Content/Plugins/jquery-ui-1.12.1/jquery-ui.min.js",
                     "~/Content/Plugins/jquery-ui-1.12.1/jquery-ui-timepicker-addon.js",
                     "~/Content/Plugins/jquery-ui-1.12.1/jquery-ui.es.js",
                     "~/Content/Plugins/Bootstrap-Duallistbox/js/jquery.bootstrap-duallistbox.min.js",
                     "~/Content/Plugins/jquery-loading-overlay/loadingoverlay.min.js",

                     // ✅ Select2 JS
                     "~/Content/Plugins/select2.min.js"
            ));
        }
    }
}
