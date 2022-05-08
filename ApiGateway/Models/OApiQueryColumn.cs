/*
' /====================================================\
'| Developed Tony N. Hyde (www.k2host.co.uk)            |
'| Projected Started: 2019-06-01                        | 
'| Use: General                                         |
' \====================================================/
*/

namespace K2host.Web.Classes
{
   
    public class OApiQueryColumn
    {

        /// <summary>
        /// The column name.
        /// </summary>
        public string Data { get; set; }
        
        /// <summary>
        ///
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///
        /// </summary>
        public bool Orderable { get; set; }

        /// <summary>
        ///
        /// </summary>
        public bool Searchable { get; set; }
       
        /// <summary>
        ///
        /// </summary>
        public OApiQuerySearch Search { get; set; }

        /// <summary>
        /// Creates the instance of the OApiJsonQueryResonse
        /// </summary>
        public OApiQueryColumn()
        {

        }

    }

}
