using CreditCard.BusinessLogic.Services;

namespace CreditCard.BusinessLogic.Factories
{
    public static class ServiceFactory
    {
        public static ICreditCardService? Create(string serviceType)
        {
            switch(serviceType)
            {
                case "CreditCardService": return new CreditCardService();
                default: return null;
            }
        }
    }
}