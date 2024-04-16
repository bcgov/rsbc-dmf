package simulation

import scenario.UserScenarios
import io.gatling.core.Predef._
import scala.concurrent.duration._

class ThrottleSimulation extends Simulation {

    val halfUserCount = (Global.userCount / 2 + 1).toInt

    setUp(
        UserScenarios.getDriverInfo.inject(constantUsersPerSec(halfUserCount) during (Global.durationInSeconds))
            .throttle(jumpToRps(Global.userCount), holdFor(Global.durationInSeconds)),
        UserScenarios.getMostRecentCase.inject(constantUsersPerSec(halfUserCount) during (Global.durationInSeconds))
            .throttle(jumpToRps(Global.userCount), holdFor(1.seconds)),
        UserScenarios.getClosedCases.inject(constantUsersPerSec(halfUserCount) during (Global.durationInSeconds))
            .throttle(jumpToRps(Global.userCount), holdFor(Global.durationInSeconds)),
        UserScenarios.getCaseDocuments.inject(constantUsersPerSec(halfUserCount) during (Global.durationInSeconds))
            .throttle(jumpToRps(Global.userCount), holdFor(Global.durationInSeconds)),
        UserScenarios.getAllDocuments.inject(constantUsersPerSec(halfUserCount) during (Global.durationInSeconds))
            .throttle(jumpToRps(Global.userCount), holdFor(Global.durationInSeconds)),
        UserScenarios.getDocumentTypes.inject(constantUsersPerSec(halfUserCount) during (Global.durationInSeconds))
            .throttle(jumpToRps(Global.userCount), holdFor(Global.durationInSeconds))
    )
        .protocols(Global.httpProtocol)
}
