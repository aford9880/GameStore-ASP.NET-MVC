using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using GameStore.Domain.Abstract;
using GameStore.Domain.Entities;
using GameStore.WebUI.Controllers;
using GameStore.WebUI.Models;
using GameStore.WebUI.HtmlHelpers;

namespace GameStore.UnitTests {
    [TestClass]
    public class UnitTest1 {

        private GamesListViewModel _result;

        void ArrangeAndAct(string curCategory, int curPage) {
            // Организация (arrange)
            Mock<IGameRepository> mock = new Mock<IGameRepository>();
            mock.Setup(m => m.Games).Returns(new List<Game> {
                new Game { GameId = 1, Name = "Игра1", Category="Cat1"},
                new Game { GameId = 2, Name = "Игра2", Category="Cat2"},
                new Game { GameId = 3, Name = "Игра3", Category="Cat1"},
                new Game { GameId = 4, Name = "Игра4", Category="Cat2"},
                new Game { GameId = 5, Name = "Игра5", Category="Cat3"}
            });
            using (GameController controller = new GameController(mock.Object)) {
                controller.pageSize = 3;
                // Действие (act)
                _result = (GamesListViewModel)controller.List(curCategory, curPage).Model;
            }                                  
        }

        [TestMethod]
        public void Can_Paginate() {
            // Начальные установки
            ArrangeAndAct(null, 2); 

            // Утверждение (assert)            
            List<Game> games = _result.Games.ToList();
            Assert.IsTrue(games.Count == 2);
            Assert.AreEqual(games[0].Name, "Игра4");
            Assert.AreEqual(games[1].Name, "Игра5");
        }

        [TestMethod]
        public void Can_Send_Pagination_View_Model() {
            // Начальные установки
            ArrangeAndAct(null, 2);

            // Assert
            PagingInfo pageInfo = _result.PagingInfo;
            Assert.AreEqual(pageInfo.CurrentPage, 2);
            Assert.AreEqual(pageInfo.ItemsPerPage, 3);
            Assert.AreEqual(pageInfo.TotalItems, 5);
            Assert.AreEqual(pageInfo.TotalPages, 2);
        }

        [TestMethod]
        public void Can_Generate_Page_Links() {

            // Организация - определение вспомогательного метода HTML - это необходимо
            // для применения расширяющего метода
            HtmlHelper myHelper = null;

            // Организация - создание объекта PagingInfo
            PagingInfo pagingInfo = new PagingInfo {
                CurrentPage = 2,
                TotalItems = 28,
                ItemsPerPage = 10
            };

            // Организация - настройка делегата с помощью лямбда-выражения

            Func<int, string> pageUrlDelegate = i => "Page" + i;            
            // или так
            //string pageUrlDelegate(int i) => "Page" + i;
            
            // Действие
            MvcHtmlString result = myHelper.PageLinks(pagingInfo, pageUrlDelegate);

            // Утверждение
            Assert.AreEqual(@"<a class=""btn btn-default"" href=""Page1"">1</a>"
                + @"<a class=""btn btn-default btn-primary selected"" href=""Page2"">2</a>"
                + @"<a class=""btn btn-default"" href=""Page3"">3</a>",
                result.ToString());
        }


        [TestMethod]
        public void Can_Filter_Games() {
            ArrangeAndAct("Cat2", 1);
            var result = _result.Games.ToList();
            // Assert            
            Assert.AreEqual(result.Count(), 2);
            Assert.IsTrue(result[0].Name == "Игра2" && result[0].Category == "Cat2");
            Assert.IsTrue(result[1].Name == "Игра4" && result[1].Category == "Cat2");
        }
    }
}