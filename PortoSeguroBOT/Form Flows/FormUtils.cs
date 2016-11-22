using Microsoft.Bot.Builder.FormFlow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PortoSeguroBOT.Form_Flows
{
    public class FormUtils
    {   
        public static IFormBuilder<T> CreateCustomForm<T>()
           where T : class
        {
            var form = new FormBuilder<T>();
            var command = form.Configuration.Commands[FormCommand.Quit];
            var terms = command.Terms.ToList();
            terms.Remove("quit");
            terms.Add("cancelar");
            terms.Add("sair");
            command.Terms = terms.ToArray();

            var templateAttribute = form.Configuration.Template(TemplateUsage.NotUnderstood);
            var patterns = templateAttribute.Patterns;
            patterns[0] += " Type *cancel* to quit or *help* if you want more information.";
            templateAttribute.Patterns = patterns;

            return form;
        }
    }
}