using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailSender
{
    /// <summary>
    /// Deals with reading different templates and creating templated data after replacing wildcards
    /// </summary>
    class TemplateManager
    {
        /// <summary>
        /// Path of template config file
        /// </summary>
        private string templateConfigPath = null;

        /// <summary>
        /// Template config instance
        /// </summary>
        private TemplateConfig templateConfig = null;

        /// <summary>
        /// Constructor which needs path of template config file
        /// </summary>
        /// <param name="templateConfigPath"></param>
        public TemplateManager(string templateConfigPath)
        {
            this.templateConfigPath = templateConfigPath;
        }

        /// <summary>
        /// - Reads the template
        /// - Update wild card elements from data table with their values
        /// </summary>
        /// <param name="templateName">Template name</param>
        /// <param name="templateInfo">DataTable which contains wild card elements and their values</param>
        /// <returns></returns>
        public string GetTemplateData(string templateName, DataTable templateInfo)
        {
            string updatedTemplateData = null;
            if (templateConfigPath != null)
            {
                if (templateConfig == null)
                {
                    templateConfig = ConfigReader.Read(templateConfigPath, typeof(TemplateConfig)) as TemplateConfig;
                    if (templateConfig != null)
                    {
                        updatedTemplateData = GetUpdatedTemplateData(GetTemplateData(GetCurrentTemplatePath(templateName)), templateInfo);
                    }
                    else
                    {
                        throw new ApplicationException("Serialization of TemplateConfig.xml failed");
                    }
                }
            }
            else
            {
                throw new ApplicationException("TemplateConfig.xml path invalid");
            }

            return updatedTemplateData;
        }

        /// <summary>
        /// Gets the path of template file for which we need the updated data
        /// </summary>
        /// <param name="templateName">Name of template whose path need to be retrieved</param>
        /// <returns>Path of template</returns>
        private string GetCurrentTemplatePath(string templateName)
        {
            string templatePath = string.Empty;
            foreach (var template in templateConfig.Templates)
            {
                if (template.name == templateName)
                {
                    templatePath = template.path;
                    break;
                }
            }

            if (string.IsNullOrEmpty(templatePath))
            {
                throw new ApplicationException("Path for " + templateName + " not found in TemplateConfig.xml");
            }
            return templatePath;
        }

        /// <summary>
        /// Reads the template data
        /// </summary>
        /// <param name="path">Path of template</param>
        /// <returns>Template Data with wild card elements</returns>
        private string GetTemplateData(string path)
        {
            return File.ReadAllText(path);
        }

        /// <summary>
        /// Update the wild cards element in template with their values
        /// </summary>
        /// <param name="templateData">Template Data</param>
        /// <param name="templateInfo">DataTable which contains wild card elements and their values</param>
        /// <returns></returns>
        private string GetUpdatedTemplateData(string templateData, DataTable templateInfo)
        {
            if (templateInfo != null && templateInfo.Rows.Count > 0)
            {
                foreach (DataRow row in templateInfo.Rows)
                {
                    string wildCard = (string)row[templateInfo.Columns[0]];
                    string value = (string)row[templateInfo.Columns[1]];
                    if (!string.IsNullOrEmpty(wildCard) && !string.IsNullOrEmpty(value))
                    {
                        templateData = templateData.Replace(wildCard, value);
                    }
                }
            }

            return templateData;
        }
    }
}
