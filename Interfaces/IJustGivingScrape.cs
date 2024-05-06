/*
 * ---------------------------------------------------------------------------------
 * "THE BEER-WARE LICENSE" (Revision 42):
 * <phk@FreeBSD.ORG> wrote this file. As long as you retain this notice you
 * can do whatever you want with this stuff. If we meet some day, and you think
 * this stuff is worth it, you can buy me a beer in return Poul-Henning Kamp
 * ---------------------------------------------------------------------------------
 *
 * This software is developed for SwimTayka, a charity dedicated to teaching 
 * swimming and water safety to underprivileged children around the globe.
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program. If not, see <http://www.gnu.org/licenses/>.
 *
 * If you reuse any part of this code, please consider making a donation to 
 * SwimTayka to support their cause. For donation information, please visit:
 * https://swimtayka.org/donate
 */

using System.Threading.Tasks;

namespace SwimTaykaStreak
{
    /// <summary>
    /// Defines a contract for a service that scrapes fundraising amounts from JustGiving pages.
    /// </summary>
    public interface IJustGivingScrape
    {
        /// <summary>
        /// Asynchronously retrieves the raised amount of money from a specified JustGiving page.
        /// </summary>
        /// <param name="url">The URL of the JustGiving page to scrape.</param>
        /// <returns>A <see cref="Task{String}"/> that represents the asynchronous operation, containing the raised amount as a string.</returns>
        Task<string> GetRaisedAmount(string url);
    }
}