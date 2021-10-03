using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.NewtonsoftJson;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace sommersoftware.dk.Models
{
    // rename properties to big letters when we have a fix.
    public class Student
    {
        protected Student() { }

        public Student(long id = default(long), string name = default(string), string tag = default(string))
        {
            this.Id = id;
            // to ensure "name" is required (not null)
            this.Name = name ?? throw new ArgumentNullException("name is a required property for Student and cannot be null");
            this.Tag = tag;
        }

        //[JsonPropertyName("id")]
        [DataMember(Name = "id", IsRequired = true, EmitDefaultValue = false)]
        public long Id { get; set; }

        [DataMember(Name = "name", IsRequired = true, EmitDefaultValue = false)]
        public string Name { get; set; }

        [DataMember(Name = "tag", EmitDefaultValue = false)]
        public string Tag { get; set; }

        public virtual string ToJson()
        {
            //return System.Text.Json.JsonSerializer.Serialize(this);
            return Newtonsoft.Json.JsonConvert.SerializeObject(this, Newtonsoft.Json.Formatting.Indented);
        }
    }
}
