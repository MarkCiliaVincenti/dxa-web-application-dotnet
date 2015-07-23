﻿using System;
using System.Collections.Generic;
using Sdl.Web.Common.Configuration;

namespace Sdl.Web.Common.Models
{
    // removed MediaObject Semantic mapping since the models using this are already specifying it with a more detailed type 
    // we can specify additional types like this, but I'm not sure what the value of that is in this case, so lets drop it
    //[SemanticEntity(Vocab = SchemaOrgVocabulary, EntityName = "MediaObject", Prefix = "s", Public = true)]
    public abstract class MediaItem : EntityModel
    {
        private const string EclMimeType = "application/externalcontentlibrary";

        private static readonly IDictionary<string, string> FontAwesomeMimeTypeToIconClassMapping = new Dictionary<string, string>
        {
            {"application/ms-excel", "excel"},
            {"application/pdf", "pdf"},
            {"application/x-wav", "audio"},
            {"audio/x-mpeg", "audio"},
            {"application/msword", "word"},
            {"text/rtf", "word"},
            {"application/zip", "archive"},
            {"image/gif", "image"},
            {"image/jpeg", "image"},
            {"image/png", "image"},
            {"image/x-bmp", "image"},
            {"text/plain", "text"},
            {"text/css", "code"},
            {"application/x-javascript", "code"},
            {"application/ms-powerpoint", "powerpoint"},
            {"video/vnd.rn-realmedia", "video"},
            {"video/quicktime", "video"},
            {"video/mpeg", "video"}
        };

        [SemanticProperty("s:contentUrl")]
        public string Url { get; set; }
        public string FileName { get; set; }
        [SemanticProperty("s:contentSize")]
        public int FileSize { get; set; }
        public string MimeType { get; set; }

        /// <summary>
        /// ECL URI for External Content Library Components (null for normal multimedia Components)
        /// </summary>
        public string EclUri
        {
            get
            {
                // TODO: ECL mimetype should actually be a real mimetype, in that case we can't use it here anymore
                if (EclMimeType.Equals(MimeType) && FileName.EndsWith(".ecl"))
                {
                    return String.Format("ecl:{0}", FileName.Replace(".ecl", String.Empty));
                }
                return null;
            }
        }

        // ECL items need to use ECL URI rather than TCM URI in XPM markup
        public override string GetXpmMarkup(Localization localization)
        {
            // TODO: ECL mimetype should actually be a real mimetype, in that case we can't use it here anymore
            if (EclMimeType.Equals(MimeType))
            {
                return base.GetXpmMarkup(localization).Replace(String.Format("tcm:{0}-{1}", localization.LocalizationId, Id), EclUri);
            }
            return base.GetXpmMarkup(localization);
        }

        /// <summary>
        /// Gets the file size with units.
        /// </summary>
        public string GetFriendlyFileSize()
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB", "PB", "EB" };
            double len = FileSize;
            int order = 0;
            while (len >= 1024 && order + 1 < sizes.Length)
            {
                order++;
                len = len / 1024;
            }

            return string.Format("{0} {1}", Math.Ceiling(len), sizes[order]);
        }

        /// <summary>
        /// Gets the name of a CSS class representing the Icon for this Media Item.
        /// </summary>
        /// <returns>The CSS class name.</returns>
        public virtual string GetIconClass()
        {
            string fileType;
            return FontAwesomeMimeTypeToIconClassMapping.TryGetValue(MimeType, out fileType) ? string.Format("fa-file-{0}-o", fileType) : "fa-file";
        }

        /// <summary>
        /// Renders an HTML representation of the Entity Model.
        /// </summary>
        /// <returns>An HTML representation.</returns>
        /// <remarks>
        /// This method is used when the Entity Model is part of a <see cref="RichText"/> instance which is mapped to a string property.
        /// In this case HTML rendering happens during model mapping, which is not ideal.
        /// Preferably, the model property should be of type <see cref="RichText"/> and the View should use @Html.DxaRichText() to get the rich text rendered as HTML.
        /// </remarks>
        public override string ToHtml()
        {
            return ToHtml("100%");
        }

        /// <summary>
        /// Renders an HTML representation of the Media Item.
        /// </summary>
        /// <param name="widthFactor">The factor to apply to the width - can be % (eg "100%") or absolute (eg "120").</param>
        /// <param name="aspect">The aspect ratio to apply.</param>
        /// <param name="cssClass">Optional CSS class name(s) to apply.</param>
        /// <param name="containerSize">The size (in grid column units) of the containing element.</param>
        /// <returns>The HTML representation.</returns>
        /// <remarks>
        /// This method is used by the <see cref="IRichTextFragment.ToHtml()"/> implementation and by the HtmlHelperExtensions.Media implementation.
        /// Both cases should be avoided, since HTML rendering should be done in View code rather than in Model code.
        /// </remarks>
        public abstract string ToHtml(string widthFactor, double aspect = 0, string cssClass = null, int containerSize = 0);
    }
}