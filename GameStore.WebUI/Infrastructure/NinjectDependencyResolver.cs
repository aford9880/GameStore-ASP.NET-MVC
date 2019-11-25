using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Mvc;
using GameStore.Domain.Abstract;
using GameStore.Domain.Concrete;
using GameStore.Domain.Entities;
using Moq;
using Ninject;

namespace GameStore.WebUI.Infrastructure {
    public class NinjectDependencyResolver : IDependencyResolver {
        private IKernel kernel;

        public NinjectDependencyResolver(IKernel kernelParam) {
            kernel = kernelParam;
            AddBindings();
        }

        public object GetService(Type serviceType) {
            return kernel.TryGet(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType) {
            return kernel.GetAll(serviceType);
        }

        private void AddBindings() {
            /*Mock<IGameRepository> mock = new Mock<IGameRepository>();
            mock.Setup(m => m.Games).Returns(new List<Game>
            {
                new Game { Name = "SimCity", Price = 1499 },
                new Game { Name = "TITANFALL", Price=2299 },
                new Game { Name = "Battlefield 4", Price=899.4M }
            });
            //Вместо создания нового экземпляра объекта реализации в
            //каждом случае ядро Ninject будет удовлетворять запросы
            //интерфейса IGameRepository одним и тем же имитированным
            //объектом.
            kernel.Bind<IGameRepository>().ToConstant(mock.Object);*/
            kernel.Bind<IGameRepository>().To<EFGameRepository>();
        }
    }
}