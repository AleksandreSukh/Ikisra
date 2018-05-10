using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.AspNet.Identity.EntityFramework;
using SharedTemplate;

namespace RestApiBase
{
    public class RegistrationForm : IRegistrationForm
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string RolesCommaSeparated { get; set; }
        public string Email { get; set; }
    }

    public interface IUtcDated
    {
        DateTime DateUtc { get; set; }
    }

    [NotMapped]
    public class Link
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public string Description { get; set; }
    }
    public abstract class LinkedResource
    {
        [NotMapped]
        public List<Link> Links { get; set; }
        [NotMapped]
        public string HRef { get; set; }
    }
    public class ApplicationUser : IdentityUser
    {
        //TODO:გვინდა?
        public string Comment { get; set; }
     
        // FirstName & LastName will be stored in a different table called MyUserInfo
        public virtual MyUserInfo MyUserInfo { get; set; }
    }

    public class MyUserInfo
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
    public class LinkedResourcePage : LinkedResource
    {
        public string Title { get; set; }

    }

    public class AuthToken : IAuthToken
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public int expires_in { get; set; }
    }

    public interface ILinkedResourceCollection<T> where T : LinkedResource
    {
        int Count { get; }
        Type ElementType { get; }
        Expression Expression { get; }
        bool IsReadOnly { get; }
        IQueryProvider Provider { get; }

        void Add(T item);
        void Clear();
        bool Contains(T item);
        void CopyTo(T[] array, int arrayIndex);
        IEnumerator<T> GetEnumerator();
        bool Remove(T item);
    }
    public abstract class LinkedResourceCollection<T> : LinkedResource, IQueryable<T>, ICollection<T>, ILinkedResourceCollection<T> where T : LinkedResource
    {
        private List<T> linkedResourceListInner;

        public LinkedResourceCollection()
        {
            this.linkedResourceListInner = new List<T>();
        }

        // Rest of the collection implementation
        public IEnumerator<T> GetEnumerator()
        {
            return linkedResourceListInner.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(T item)
        {
            linkedResourceListInner.Add(item);
        }

        public void Clear()
        {
            linkedResourceListInner.Clear();
        }

        public bool Contains(T item)
        {
            return linkedResourceListInner.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            linkedResourceListInner.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            return linkedResourceListInner.Remove(item);
        }

        public int Count => linkedResourceListInner.Count;
        public bool IsReadOnly => (linkedResourceListInner as IList<T>).IsReadOnly;

        public Expression Expression => linkedResourceListInner.AsQueryable().Expression;

        public Type ElementType => linkedResourceListInner.AsQueryable().ElementType;

        public IQueryProvider Provider => linkedResourceListInner.AsQueryable().Provider;
    }

}
