using System;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json;

namespace ToDoXamarinDemo
{
	public class TodoItem
	{
		string id;
		string name;
		bool done;
        string image;

		[JsonProperty(PropertyName = "id")]
		public string Id
		{
			get { return id; }
			set { id = value;}
		}

		[JsonProperty(PropertyName = "text")]
		public string Name
		{
			get { return name; }
			set { name = value;}
		}

		[JsonProperty(PropertyName = "complete")]
		public bool Done
		{
			get { return done; }
			set { done = value;}
		}

        [JsonProperty(PropertyName = "image")]
        public string Image
        {
            get { return image; }
            set { image = value; }
        }

        [Version]
        public string Version { get; set; }
	}
}

