using DinkToPdf;
using DinkToPdf.Contracts;
using HandlebarsDotNet;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Newtonsoft.Json;
using PdfGenerator.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace PdfGenerator.Formatters
{
    public class PdfFormatter : OutputFormatter
    {
        private readonly IConverter htmlToPdfConverter;
        private readonly Dictionary<string, string> templates;

        public PdfFormatter(IConverter htmlToPdfConverter, IDictionary<string, string> templates)
        {
            this.htmlToPdfConverter = htmlToPdfConverter;
            this.templates =new Dictionary<string, string>( templates);
            SupportedMediaTypes.Add(Microsoft.Net.Http.Headers.MediaTypeHeaderValue.Parse("application/pdf"));
        }
        protected override bool CanWriteType(Type type)
        {

            if (typeof(IOutputData).IsAssignableFrom(type) )
            {
                return base.CanWriteType(type);
            }
            return false;
        }

        public override Task WriteResponseBodyAsync(OutputFormatterWriteContext context)
        {
            var response = context.HttpContext.Response;
            if (!context.HttpContext.Request.Query.ContainsKey("templateName")|| !templates.ContainsKey(context.HttpContext.Request.Query["templateName"][0].ToUpper()))
            {
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                response.ContentType = "application/json";
                return response.WriteAsync(JsonConvert.SerializeObject(new { ErrCode = "TEMPLATE_NOT_FOUND", ErrDesc = "Template not defined or template name missing in Query String" }));


            }
            var templateKey = context.HttpContext.Request.Query["templateName"][0].ToUpper();
           

          
            var result = BuildHTML(context.Object, templateKey);
            var doc = new HtmlToPdfDocument()
            {
                GlobalSettings = {
                    ColorMode = ColorMode.Color,
                    PaperSize = PaperKind.A4,
                    Orientation = Orientation.Landscape,
                    Margins = new MarginSettings(20, 20, 20, 20)
                },

                Objects = {
                    new ObjectSettings()
                    {
                        HtmlContent =result,
                        PagesCount = true,
                        FooterSettings = { FontSize = 9, Left = "\n This report was generated on " + DateTime.Now, Right = "\n Page [page] of [toPage]", Line = true, Spacing = 2.812 },
                        WebSettings = new WebSettings() { MinimumFontSize = 12, DefaultEncoding = "utf-8" }
                    }
                }
            };
           
            System.Net.Mime.ContentDisposition cd = new System.Net.Mime.ContentDisposition
            {
                FileName = templateKey + ".pdf",
                Inline = false  // false = prompt the user for downloading;  true = browser to try to show the file inline
            };
            byte[] pdf = htmlToPdfConverter.Convert(doc);
            response.StatusCode = (int)HttpStatusCode.OK;
            response.ContentType = "application/pdf";
            response.Headers.Add("Content-Disposition", cd.ToString()); //$"attachment; filename={fileName}"                                        
            return response.Body.WriteAsync(pdf, 0, pdf.Length);
        }

        private string BuildHTML(object @object, string templateKey)
        {
        
            var template = Handlebars.Compile(templates[templateKey]);

            var result = template(@object);

            return result;
        }
    }
}

