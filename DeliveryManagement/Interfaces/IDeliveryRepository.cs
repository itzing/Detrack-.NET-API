using System;
using System.Collections.Generic;
using System.Drawing;
using Detrack.Model.Deliveries;

namespace Detrack.Data.Interfaces
{
	public interface IDeliveryRepository
	{
		IEnumerable<Delivery> GetAllForDate(DateTime date);
		Delivery GetDelivery(DateTime deliveryDate, string deliveryDo);
		IEnumerable<Delivery> GetDeliveries(IEnumerable<Delivery> deliveries);

		void Add(List<Delivery> deliveries);
		void EditDeliveries(List<Delivery> deliveries);
		void DeleteDeliveriesForDate(DateTime date);

		Image GetSignatureImage(Delivery delivery);
	}
}
