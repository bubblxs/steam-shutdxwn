using System.Text.Json;
using steam_shutdxwn.Source.Classes;

namespace test_steam_shutdxwn
{
    [TestClass]
    public class Tests_SteamShutdxwn
    {
        private string? _path = "/path/to/acf";

        [TestMethod]
        public void ActToJson()
        {
            try
            {
                string[] files = Directory.GetFiles(_path, "*.acf");

                foreach (string f in files)
                {
                    string? content = Acf.ToJson(f);
                    string fileName = f.Split("\\").Last();

                    //files that starts with err_ should return null.
                    if (fileName.StartsWith("err"))
                    {
                        Assert.IsNull(content, $"$File: {f}\n.Its content: {content}");
                    }
                    else
                    {
                        Acf? acf = JsonSerializer.Deserialize<Acf>(content);
                        Assert.IsNotNull(acf, $"$File: {f}\n.Its content: {content}");
                    }
                }
            }
            catch (Exception ex)
            {
                Assert.Fail($"Error: {ex.Message}");
            }
        }

        [TestMethod]
        public void Success_AcfToJson()
        {
            try
            {
                string[] files = Directory.GetFiles(_path, "app*.acf");

                foreach (string f in files)
                {
                    string? content = Acf.ToJson(f);
                    Acf? acf = JsonSerializer.Deserialize<Acf>(content);
                    Game? game = new Game()
                    {
                        StateFlag = acf.StateFlags,
                        Name = acf.name,
                        AppId = acf.appid,
                    };
                    Assert.IsNotNull(acf, $"$File: {f}\n.Its content: {content}");
                    Assert.IsTrue(game.StateFlag.Length > 0, $"{game.StateFlag}");
                    Assert.IsTrue(game.Name.Length > 0, $"{game.Name}");
                    Assert.IsTrue(game.AppId.Length > 0, $"{game.AppId}");
                }
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod]
        public void Error_AcfToJson()
        {
            try
            {
                string[] files = Directory.GetFiles(_path, "err_*.acf");

                foreach (string f in files)
                {
                    string? content = Acf.ToJson(f);
                    Assert.IsNull(content, $"$File: {f}\n.Its content: {content}");
                }
            }
            catch (Exception ex)
            {
                Assert.Fail($"Error: {ex.Message}");
            }
        }
    }
}