using Microsoft.AspNetCore.Razor.TagHelpers;

namespace NetCoreIdentityApp.Web.TagHelpers
{
    public class UserPictureTagHelper : TagHelper
    {
        public string PictureUrl { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "img";
            if (string.IsNullOrEmpty(PictureUrl))
            {
                output.Attributes.SetAttribute("src", "/userpictures/default_picture.png");
            }
            else
            {
                output.Attributes.SetAttribute("src", $"/userpictures/{PictureUrl}");

            }
            base.Process(context, output);
        }
    }
}
