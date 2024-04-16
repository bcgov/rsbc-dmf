package simulation

import scenario.UserScenarios
import io.gatling.core.Predef._
import scala.concurrent.duration._

class RampUserSimulation extends Simulation {

    setUp(
        UserScenarios.getDriverInfo.inject(rampUsers(Global.userCount) during (Global.durationInSeconds)),
        UserScenarios.getMostRecentCase.inject(rampUsers(Global.userCount) during (Global.durationInSeconds)),
        UserScenarios.getClosedCases.inject(rampUsers(Global.userCount) during (Global.durationInSeconds)),
        UserScenarios.getCaseDocuments.inject(rampUsers(Global.userCount) during (Global.durationInSeconds)),
        UserScenarios.getAllDocuments.inject(rampUsers(Global.userCount) during (Global.durationInSeconds)),
        UserScenarios.getDocumentTypes.inject(rampUsers(Global.userCount) during (Global.durationInSeconds))
    )
        .protocols(Global.httpProtocol)
}
