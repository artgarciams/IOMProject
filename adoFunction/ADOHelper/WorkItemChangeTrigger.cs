using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADOPipeline.ADO.Helper
{
    
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class Message
    {
        public string text { get; set; }
        public string html { get; set; }
        public string markdown { get; set; }
    }

    public class DetailedMessage
    {
        public string text { get; set; }
        public string html { get; set; }
        public string markdown { get; set; }
    }

    public class Avatar
    {
        public string href { get; set; }
    }

    public class Links
    {
        public Avatar avatar { get; set; }
        public Self self { get; set; }
        public WorkItemUpdates workItemUpdates { get; set; }
        public Parent parent { get; set; }
        public Html html { get; set; }
        public WorkItemRevisions workItemRevisions { get; set; }
    }

    public class RevisedBy
    {
        public string id { get; set; }
        public string name { get; set; }
        public string displayName { get; set; }
        public string url { get; set; }
        public Links _links { get; set; }
        public string uniqueName { get; set; }
        public string imageUrl { get; set; }
        public string descriptor { get; set; }
    }

    public class SystemRev
    {
        public int oldValue { get; set; }
        public int newValue { get; set; }
    }

    public class SystemAuthorizedDate
    {
        public DateTime oldValue { get; set; }
        public DateTime newValue { get; set; }
    }

    public class SystemRevisedDate
    {
        public DateTime oldValue { get; set; }
        public DateTime newValue { get; set; }
    }

    public class SystemChangedDate
    {
        public DateTime oldValue { get; set; }
        public DateTime newValue { get; set; }
    }

    public class SystemWatermark
    {
        public int oldValue { get; set; }
        public int newValue { get; set; }
    }

    public class SystemDescription
    {
        public string oldValue { get; set; }
        public string newValue { get; set; }
    }

    public class Fields
    {
        
    }

    public class Self
    {
        public string href { get; set; }
    }

    public class WorkItemUpdates
    {
        public string href { get; set; }
    }

    public class Parent
    {
        public string href { get; set; }
    }

    public class Html
    {
        public string href { get; set; }
    }

    public class Attributes
    {
        public DateTime authorizedDate { get; set; }
        public int id { get; set; }
        public DateTime resourceCreatedDate { get; set; }
        public DateTime resourceModifiedDate { get; set; }
        public DateTime revisedDate { get; set; }
        public string comment { get; set; }
    }

    public class Relation
    {
        public string rel { get; set; }
        public string url { get; set; }
        public Attributes attributes { get; set; }
    }

    public class WorkItemRevisions
    {
        public string href { get; set; }
    }

    public class Revision
    {
        public int id { get; set; }
        public int rev { get; set; }
        public Fields fields { get; set; }
        public List<Relation> relations { get; set; }
        public Links _links { get; set; }
        public string url { get; set; }
    }

    public class Resource
    {
        public int id { get; set; }
        public int workItemId { get; set; }
        public int rev { get; set; }
        public RevisedBy revisedBy { get; set; }
        public DateTime revisedDate { get; set; }
        public Fields fields { get; set; }
        public Links _links { get; set; }
        public string url { get; set; }
        public Revision revision { get; set; }
    }

    public class Collection
    {
        public string id { get; set; }
        public string baseUrl { get; set; }
    }

    public class Account
    {
        public string id { get; set; }
        public string baseUrl { get; set; }
    }

    public class Project
    {
        public string id { get; set; }
        public string baseUrl { get; set; }
    }

    public class ResourceContainers
    {
        public Collection collection { get; set; }
        public Account account { get; set; }
        public Project project { get; set; }
    }

    public class WorkItemChangeTrigger
    {
        public string id { get; set; }
        public string eventType { get; set; }
        public string publisherId { get; set; }
        public Message message { get; set; }
        public DetailedMessage detailedMessage { get; set; }
        public Resource resource { get; set; }
        public string resourceVersion { get; set; }
        public ResourceContainers resourceContainers { get; set; }
        public DateTime createdDate { get; set; }
    }


}
