
using System;

namespace M3alam.Domain.Entities
{
    public class Provider
    {
        public string Id { get; set; }
        public string Bio { get; set; }
        public int Experience { get; set; }
        public string Skills { get; set; }

        public void CreateRequest() { /* create request logic */ }
        public void CancelRequest() { /* cancel request logic */ }
        public void SelectProvider() { /* select provider logic */ }
        public void RateProvider() { /* rate provider logic */ }
    }
}
